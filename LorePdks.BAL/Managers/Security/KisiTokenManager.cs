using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using LorePdks.COMMON.DTO.Security.Auth;
using LorePdks.COMMON.Configuration;
using LorePdks.COMMON.DTO.Security.Auth;
using LorePdks.COMMON.DTO.Security.User;
using LorePdks.COMMON.Extensions;
using LorePdks.COMMON.Models;
using LorePdks.DAL.Model;
using LorePdks.DAL.Repository;
using System.Security.Cryptography;
using System.Text;
using LorePdks.COMMON.Enums;
using LorePdks.BAL.Managers.Security.Interfaces;
using LorePdks.BAL.Managers.Helper.Interfaces;
using LorePdks.BAL.Managers.Common.Kod.Interfaces;
using LorePdks.COMMON.Logging;

namespace LorePdks.BAL.Managers.Security
{
    public class KisiTokenManager : IKisiTokenManager
    {
        private const string _alg = "HmacSHA256"; //bu algoritma name bu değişmeycek
        private const string _salt = "ae8ua8vt2FBXphj9WQfvFh";

        private IKodManager _kodManager;
        private IMapper _mapper;
        private IHelperManager _helperManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMenuManager _menuManager;
        private readonly IWidgetManager _widgetManager;
        private readonly IPermissionManager _permissionManager;
        private readonly IPageManager _pageManager;


        public KisiTokenManager(
            IMapper mapper,
            IKodManager kodManager,
            IHelperManager helperManager,
            IHttpContextAccessor httpContextAccessor,

            IMenuManager menuManager,
            IWidgetManager widgetManager,
            IPermissionManager permissionManager,
            IPageManager pageManager

            )
        {
            _mapper = mapper;
            _kodManager = kodManager;
            _helperManager = helperManager;
            _httpContextAccessor = httpContextAccessor;

            _menuManager = menuManager;
            _widgetManager = widgetManager;
            _permissionManager = permissionManager;
            _pageManager = pageManager;

        }

        public T_Pos_AUTH_USER_TOKEN getKisiTokenByToken(string appToken)
        {
            var repoKisiToken = new GenericRepository<T_Pos_AUTH_USER_TOKEN>();
            var data = repoKisiToken.Get("EK_ODEME_TOKEN =@token", new { token = appToken });
            return data;
        }

        public T_Pos_AUTH_USER_TOKEN_CLOB getKisiTokenClobByKisiTokenId(long kisiTokenId)
        {
            var repoKisiToken = new GenericRepository<T_Pos_AUTH_USER_TOKEN_CLOB>();
            var data = repoKisiToken.Get("KISI_TOKEN_ID =@ktId", new { ktId = kisiTokenId });
            return data;
        }

        /// <summary>
        /// sso dan token alındıktan sonra infoya gidip bilgi sorulur o bilginin sonucu buraya döndürülerek kendi uygulama içi token ımız alınır 
        /// </summary>
        /// <param name="ssoEntity"></param>
        /// <returns></returns>
        public LoginResponseDTO getLoginBySSODto(LoginWithSSOResponseDTO ssoEntity)
        {

            LoginResponseDTO loginResponseDto = new LoginResponseDTO();

            var repoUser = new GenericRepository<T_Pos_AUTH_USER>(); // TODO: modleler gelince düzelt T_gen_Kisi
            var repoUserToken = new GenericRepository<T_Pos_AUTH_USER_TOKEN>();

            DateTime expDate = DateTime.Now.AddMinutes(Convert.ToDouble(CoreConfig.TokenCreateMin));

            var kisi = repoUser.Get("LOGIN_NAME=@p1", new { p1 = ssoEntity.loginName });

            if (kisi == null || kisi.ID == 0)
            {
                AppLogService.Instance.SetLogIndexTip(AppEnums.LOG_INDEX_TIP.LOGIN).Error($"Db de kisi kaydı bulunamadı aranan login name:{ssoEntity.loginName}");

                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, "Bu kullanici adi için kişi bulunamadı.");
            }

            // önce geçmiş tokenlar temizlensin
            if (ssoEntity.isYerineLogin)
            {
                DeleteKisiTokenByLoginName(ssoEntity.loginName, AppEnums.KISI_TOKEN_DELETE_REASON.YERINE_LOGIN_OLUNMASI_SEBEBI_ILE);
            }
            else
            {
                DeleteKisiTokenByLoginName(ssoEntity.loginName, AppEnums.KISI_TOKEN_DELETE_REASON.YENIDEN_LOGIN_SEBEBI_ILE);
            }


            string appToken = generateToken(ssoEntity.loginName);
            string appRefreshToken = generateToken(appToken);

            T_Pos_AUTH_USER_TOKEN kt = new T_Pos_AUTH_USER_TOKEN();
            kt.USER_ID = kisi.ID;
            kt.AD_SOYAD = ssoEntity.adSoyad;
            kt.LOGIN_ARACI_UYGULAMA_TOKEN = ssoEntity.ssoToken; //Bu SSO den gelen token 
            //kt.LOGIN_ARACI_UYGULAMA_TIP_KID = (int)AppEnums.LOGIN_ARACI_KURUM_TIP.SSO;
            kt.KIMLIK_NO = ssoEntity.kimlikNo;
            kt.LOGIN_NAME = ssoEntity.loginName;
            kt.APP_TOKEN = appToken;
            kt.USER_AGENT = _helperManager.GetUserAgent();
            kt.EXP_DATE = expDate;
            kt.IP = _helperManager.GetIPAddress();
            if (ssoEntity.isYerineLogin)
            {
                kt.AD_SOYAD = kisi.AD + " " + kisi.SOYAD + "(" + ssoEntity.adSoyad + ")";
                kt.KIMLIK_NO = kisi.KIMLIK_NO;
                kt.YERINE_LOGIN_ADMIN_LOGIN_NAME = ssoEntity.yerineLoginOlanAdminUserName;
                kt.YERINE_LOGIN_ADMIN_LOGIN_TOKEN = ssoEntity.yerineLoginOlanAdminAppToken; // bu bizim o kişi için üretmiş olduğumuz uygulama token ımız sso filan değil yani
                kt.IS_YERINE_LOGIN = 1;//true;
            }
            //var ogrenciManager = new OgrenciManager(_mapper);


            if (string.IsNullOrEmpty(kt.IP) || kt.IP.Length < 7)//string.IsNullOrEmpty(kt.IP_ADRES) || kt.IP_ADRES.Length < 7
            {
                AppLogService.Instance.SetLogIndexTip(AppEnums.LOG_INDEX_TIP.LOGIN).Error($"Ip adresi alnamadı.Bu nedenle login olunamaz:{ssoEntity.loginName}");

                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, "Ip adresi alnamadı.Bu nedenle login olunamaz");

            }


            string tokenName = CoreConfig.TokenKeyName;
            _httpContextAccessor.HttpContext.Request.Headers[tokenName] = appToken;


            //TODO: modeller gelince kişi dto yu düzelt 
            UserDTO userDto = _mapper.Map<T_Pos_AUTH_USER, UserDTO>(kisi);

            //sadece prodta yerine login için ayrı bir kontrol koymak istersen aşağıya yaz
            if (CoreConfig.IsProd && ssoEntity.isYerineLogin)
            {

            }


            repoUserToken.Save(kt);

            UserTokenDTO ktDto = new UserTokenDTO();
            ktDto.ip = kt.IP;
            ktDto.isLogin = true;
            ktDto.loginAraciUygulamaToken = ssoEntity.ssoToken;
            //ktDto.loginAraciUygulamaTipKodDto = _kodManager.GetKodDtoByKodId(Convert.ToInt32(AppEnums.LOGIN_ARACI_KURUM_TIP.SSO));
            ktDto.appToken = kt.APP_TOKEN;
            ktDto.userTypes = new List<string>();
            ktDto.expireDate = Convert.ToDateTime(kt.EXP_DATE);
            ktDto.id = kt.ID;
            ktDto.userDto = userDto;
            ktDto.loginName = ssoEntity.loginName;


            ktDto.yerineLoginAdminLoginName = kt.YERINE_LOGIN_ADMIN_LOGIN_NAME;
            ktDto.isYerineLogin = Convert.ToBoolean(kt.IS_YERINE_LOGIN);


            //loginResponseDTO.userDto = userDto;
            loginResponseDto.userTokenDto = ktDto;

            var ktDtoForSerilize = ktDto.Clone();
            //volki burayıda aç rollerden sonra
            //ktDtoForSerilize.actionPermissionDto = actionPermissionDtoList;
            if (ktDtoForSerilize.userDto != null)
            {
                //bu alan çok yer kaplıyor db de tutmamak gerek.
                ktDtoForSerilize.userDto.base64FotoUrl = "";
            }

            //kişi token jsonu oluştur ve değerleri yaz - başla
            string jsonString = JsonConvert.SerializeObject(ktDtoForSerilize);
            var repoKisiTokenClob = new GenericRepository<T_Pos_AUTH_USER_TOKEN_CLOB>();
            T_Pos_AUTH_USER_TOKEN_CLOB tokenClb = new T_Pos_AUTH_USER_TOKEN_CLOB
            {
                KISI_TOKEN_ID = kt.ID,
                LOGIN_DTO_JSON = jsonString
            };

            repoKisiTokenClob.Save(tokenClb);


            kt.LOGIN_DTO_JSON_CLOB_ID = tokenClb.ID;
            repoUserToken.Save(kt);




            loginResponseDto.menuListDto = _menuManager.GetMenuListByUserId(userDto.id);
            if (!(loginResponseDto.menuListDto.Count > 0))
            {
                throw new AppException(500, "Yetkiniz bululmamaktadır. Login olamazsınız");
            }
            loginResponseDto.widgetListDto = _widgetManager.GetWidgetListByUserId(userDto.id);
            loginResponseDto.permissionListDto = _permissionManager.GetPermissionListByUserId(userDto.id);
            loginResponseDto.pageListDto = _pageManager.GetPageListByUserId(userDto.id);


            //kişi token jsonu oluştur ve değerleri yaz - bitti
            //volki yetkilei nereye dolduracaksan bas burada ae
            //List<UserActionPermissionDTO> actionPermissionDtoList = new List<UserActionPermissionDTO>();
            //foreach (var item in kisiOgrenciDtoList)
            //{
            //
            //    item.menuListDto = _menuManager.GetMenuListByOgrenciId(item.id);
            //    item.PageListDto = _menuManager.GetPageListByOgrenciId(item.id);
            //    item.userCustomPermissionDto = _persmissionManager.GetCustomPermissionsByOgrenciId(item.id);
            //    actionPermissionDtoList.AddRange(_persmissionManager.GetControllerActionPermissionByOgrenciId(item.id));
            //
            //}
            // volki ilk seçili yetki gibi bir durum için burayı yazmışsın sanırım bunuda bas ae
            //if (kisiOgrenciDtoList.Count() <= 0)
            //{
            //    actionPermissionDtoList.AddRange(_persmissionManager.GetControllerActionPermissionByKisiId(kisi.ID));
            //}

            //kisiOgrenciDtoList = kisiOgrenciDtoList.OrderBy(x => x.ogrenciStatuKodDto.sira).ThenBy(c => c.birimDto.ustBirimDto.ad).ToList();
            //userDto.userOgrenciDtoList = kisiOgrenciDtoList;


            return loginResponseDto;

        }

        public UserTokenDTO GetKisiTokenDtoByToken(string token)
        {

            var repoKisiTokenClob = new GenericRepository<T_Pos_AUTH_USER_TOKEN_CLOB>();

            var kisiToken = getKisiTokenByToken(token);

            if (kisiToken == null || kisiToken.ID == 0)
            {
                throw new AppException(MessageCode.ERROR_501_YENIDEN_LOGIN_OLMALI, "login bilgilerinizde problem tespit edildi lütfen yeniden login olunuz.");
            }

            var tokenClobData = kisiToken == null ? null : repoKisiTokenClob.Get((long)kisiToken.LOGIN_DTO_JSON_CLOB_ID);

            UserTokenDTO userTokenDto = JsonConvert.DeserializeObject<UserTokenDTO>(tokenClobData.LOGIN_DTO_JSON);

            if (userTokenDto == null || userTokenDto.id == 0)
            {
                AppLogService.Instance.SetLogIndexTip(AppEnums.LOG_INDEX_TIP.LOGIN).Error($"login bilgilerinizde problem tespit edildi lütfen yeniden login olnuz.userTokenDto json db okunup düzgün çevrilemedi LOGIN_DTO_JSON_CLOB_ID:{(int)kisiToken.LOGIN_DTO_JSON_CLOB_ID}");

                throw new AppException(MessageCode.ERROR_501_YENIDEN_LOGIN_OLMALI, "login bilgilerinizde problem tespit edildi lütfen yeniden login olnuz.");
            }

            return userTokenDto;
        }

        public void DeleteKisiTokenByLoginName(string loginName, AppEnums.KISI_TOKEN_DELETE_REASON deleteSebepKod, string deleteSebepAciklama = "")
        {
            //buraya girilmişse tüm geçmiş kisitokenlari sil  kişi belli olmadığındna helpersız save
            var repoKisiToken = new GenericRepository<T_Pos_AUTH_USER_TOKEN>();
            var list = repoKisiToken.GetList("( LOGIN_NAME =@p1 OR YERINE_LOGIN_ADMIN_LOGIN_NAME=@p1 )", new { p1 = loginName }).ToList();

            foreach (var item in list)
            {
                item.DELETE_REASON_KOD_ID = Convert.ToInt32(deleteSebepKod);
                item.DELETE_REASON = string.IsNullOrEmpty(deleteSebepAciklama) ? deleteSebepKod.ToString() : deleteSebepAciklama;
                item.ISDELETED = true;//true;
                item.SERVER_INFO = "";

                // yerine login olan mı siliniyor
                if (Convert.ToBoolean(item.IS_YERINE_LOGIN))
                {
                    //yerine loginli bir veri silinecekse ve aşağıdaki ife giriyorsa bu durumda sadece öğrencinin kendi giriş yaptıkları temizlensin yerine loginler temizlenmesin eğer admin girdiyse adminin giriş yaparken temizlensin eğer kilitleme gerekirse burada ek kontroller yapacağız
                    if (loginName != item.YERINE_LOGIN_ADMIN_LOGIN_NAME)
                    {
                        continue;
                    }
                }

                repoKisiToken.Save(item);


            }
        }

        public void DeleteKisiTokenByToken(string token, AppEnums.KISI_TOKEN_DELETE_REASON deleteSebepKod, string deleteSebepAciklama = "")
        {
            //buraya girilmişse tüm geçmiş kisitokenlari sil
            var repoKisiToken = new GenericRepository<T_Pos_AUTH_USER_TOKEN>();
            var list = repoKisiToken.GetList("EK_ODEME_TOKEN = @token", new { token }).ToList();

            foreach (var item in list)
            {
                item.DELETE_REASON_KOD_ID = (int)deleteSebepKod;
                item.DELETE_REASON = string.IsNullOrEmpty(deleteSebepAciklama) ? deleteSebepKod.ToString() : deleteSebepAciklama;
                item.ISDELETED = true;//true;

                repoKisiToken.Save(item);

            }
        }

        public void Logout(string token)
        {
            var ktDto = GetKisiTokenDtoByToken(token);

            if (ktDto.isYerineLogin)
            {
                DeleteKisiTokenByLoginName(ktDto.yerineLoginAdminLoginName, AppEnums.KISI_TOKEN_DELETE_REASON.KENDI_ISTEGI_SEBEBI_ILE);
            }
            else
            {
                DeleteKisiTokenByLoginName(ktDto.loginName, AppEnums.KISI_TOKEN_DELETE_REASON.KENDI_ISTEGI_SEBEBI_ILE);
            }

        }

        public UserTokenDTO tokenValidate(string token)
        {

            // var kt = getKisiTokenByToken(token);
            var userTokenDto = GetKisiTokenDtoByToken(token);

            if (string.IsNullOrEmpty(token))
            {
                AppLogService.Instance.SetLogIndexTip(AppEnums.LOG_INDEX_TIP.LOGIN).Error($"token valide edilirken token okunamadı.");

                throw new AppException(MessageCode.ERROR_501_YENIDEN_LOGIN_OLMALI, "token okunamadı");
            }

            if (userTokenDto == null || userTokenDto.id == 0)
            {
                DeleteKisiTokenByToken(token, AppEnums.KISI_TOKEN_DELETE_REASON.KONTROLDEN_GECMEDIGI_ICIN, "tokenValidate metodunda token a ait loginDto bulunamadı.");

                AppLogService.Instance.SetLogIndexTip(AppEnums.LOG_INDEX_TIP.LOGIN).Error($"Login bilginiz artık geçersiz.Lütfen yeniden giriş yapınız. {AppEnums.KISI_TOKEN_DELETE_REASON.KONTROLDEN_GECMEDIGI_ICIN.ToString()}");

                throw new AppException(MessageCode.ERROR_501_YENIDEN_LOGIN_OLMALI, "Login bilginiz artık geçersiz. Lütfen yeniden giriş yapınız.");
            }


            if (DateTime.Now > userTokenDto.expireDate)
            {
                DeleteKisiTokenByToken(token, AppEnums.KISI_TOKEN_DELETE_REASON.GECERLILIK_TARIHI_DOLMASI_SEBEBI_ILE);

                AppLogService.Instance.SetLogIndexTip(AppEnums.LOG_INDEX_TIP.LOGIN).Error($"Login bilginiz artık geçersiz.Lütfen yeniden giriş yapınız. {AppEnums.KISI_TOKEN_DELETE_REASON.GECERLILIK_TARIHI_DOLMASI_SEBEBI_ILE.ToString()}");

                throw new AppException(MessageCode.ERROR_501_YENIDEN_LOGIN_OLMALI, "Login bilginiz artık geçersiz. Lütfen yeniden giriş yapınız.");
            }

            // expdate son sonDEgerlemeDak içindeyse dk içindeyse exp date yi güncelle değerler configte var

            int kontrolDk = Convert.ToInt32(CoreConfig.TokenExpMin);
            int eklenecekDk = Convert.ToInt32(CoreConfig.TokenExpAddMin);

            if ((userTokenDto.expireDate - DateTime.Now).TotalMinutes <= kontrolDk)
            {
                userTokenDto = saveTokenExpDateUzat(token, eklenecekDk);

            }

            return userTokenDto;
        }

        public UserTokenDTO saveTokenExpDateUzat(string token, int eklenecekDk)
        {

            var kt = getKisiTokenByToken(token);
            var ktc = getKisiTokenClobByKisiTokenId(kt.ID);
            var userTokenDto = GetKisiTokenDtoByToken(token);

            userTokenDto.expireDate = userTokenDto.expireDate.AddMinutes(eklenecekDk);
            //userTokenDto.userDto.base64FotoUrl = ""; // bu alan db ye kaydedilmemeli çok gereksiz büyütüyor.
            var ktcDtoForSerilize = userTokenDto.Clone();
            if (ktcDtoForSerilize.userDto != null)
            {
                ktcDtoForSerilize.userDto.base64FotoUrl = "";
            }
            //kişi token jsonu oluştur ve değerleri yaz - başla
            string jsonString = JsonConvert.SerializeObject(ktcDtoForSerilize);

            ktc.LOGIN_DTO_JSON = jsonString;

            kt.EXP_DATE = userTokenDto.expireDate;

            var repoKisiToken = new GenericRepository<T_Pos_AUTH_USER_TOKEN>();
            var repoKisiTokenClob = new GenericRepository<T_Pos_AUTH_USER_TOKEN_CLOB>();

            repoKisiToken.Save(kt);
            repoKisiTokenClob.Save(ktc);

            return userTokenDto;
        }

        public string generateToken(string userName)
        {
            Guid guid = Guid.NewGuid();
            string guidStr = Convert.ToString(guid);

            string ip = _helperManager.GetIPAddress();
            string userAgent = _helperManager.GetUserAgent();


            string hash = string.Join(":", new string[] { userName, ip, userAgent, Convert.ToString(guidStr) });
            string hashLeft = "";
            string hashRight = Convert.ToBase64String(Encoding.UTF8.GetBytes(userName));
            //if (hashRight.Length > 3)
            //    hashRight = hashRight.Substring(0, hashRight.Length - 3);

            using (HMAC hmac = HMAC.Create(_alg))
            {
                hmac.Key = Encoding.UTF8.GetBytes(guidStr);
                hmac.ComputeHash(Encoding.UTF8.GetBytes(hash));
                hashLeft = Convert.ToBase64String(hmac.Hash);
                // hashRight = string.Join(":", new string[] { userName, guidStr });
            }

            string token = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Join(":", hashLeft, hashRight)));

            token = token
            .Replace('+', '-') // replace URL unsafe characters with safe ones
            .Replace('/', '_') // replace URL unsafe characters with safe ones
             .Replace("=", ""); // no padding
            ;



            return token;


        }

        public string GetHashedPassword(string password)
        {
            string key = string.Join(":", new string[] { password, _salt });
            using (HMAC hmac = HMAC.Create(_alg))
            {
                // Hash the key.
                hmac.Key = Encoding.UTF8.GetBytes(_salt);
                hmac.ComputeHash(Encoding.UTF8.GetBytes(key));
                return Convert.ToBase64String(hmac.Hash);
            }
        }

        //public LoginResponseDTO getLoginWithoutKisiByIdmDto(IdmLoginValidationResponceDTO idmDto)
        //{
        //    LoginResponseDTO loginResponseDTO = new LoginResponseDTO();

        //    var repoKisiToken = new GenericRepository<T_oys_KisiToken>();

        //    DateTime expDate = DateTime.Now.AddMinutes(Convert.ToDouble(CoreConfig.TokenCreateMin));

        //    var repoKisi = new GenericRepository<T_gen_Kisi>();
        //    var kisi = repoKisi.Get("LoginName=@loginName", new { loginName = idmDto.UserName });

        //    if (kisi == null || kisi.ID == 0)
        //    {
        //        AppLogService.Instance.SetLogIndexTip(AppEnums.LOG_INDEX_TIP.LOGIN).Error($"Db de kisi kaydı bulunamadı aranan login name:{idmDto.UserName}");

        //        throw new AppException(MessageCode.ERROR_500, "Bu kullanici adi için kişi bulunamadı.");
        //    }
        //    UserDTO userDto = _modelDtoConverterHelper.T_gen_KisiToUserDTO(kisi);

        //    var kt = repoKisiToken.GetList("( LOGIN_NAME = @loginName OR YERINE_LOGIN_ADMIN_LOGIN_NAME=@loginName )", new { loginName = idmDto.UserName })
        //        .ToList()
        //        .OrderByDescending(x => x.ID)
        //        .FirstOrDefault();
        //    String oysToken = kt?.OYS_TOKEN ?? "";
        //    String oysRefreshToken = kt?.OYS_REFRESH_TOKEN ?? "";
        //    if (kt == null || kt.EXP_DATE < DateTime.Now)
        //    {
        //        DeleteKisiTokenByLoginName(idmDto.UserName, AppEnums.KISI_TOKEN_DELETE_REASON.YENIDEN_LOGIN_SEBEBI_ILE);
        //        oysToken = generateToken(idmDto.UserName);
        //        oysRefreshToken = generateToken(oysToken);
        //        kt = new T_oys_KisiToken();
        //        kt.AD_SOYAD = idmDto.Name + " " + idmDto.SurName;
        //        kt.IDM_TOKEN = idmDto.Token; //Bu idm den gelen token 
        //        kt.KIMLIK_NO = idmDto.KimlikNo;
        //        kt.LOGIN_NAME = idmDto.UserName;
        //        kt.OYS_TOKEN = oysToken;
        //        kt.USER_AGENT = _helperManager.GetUserAgent();
        //        kt.EXP_DATE = expDate;
        //        kt.IP_ADRES = _helperManager.GetIPAddress();



        //        if (string.IsNullOrEmpty(kt.IP_ADRES) || kt.IP_ADRES.Length < 7)
        //        {
        //            AppLogService.Instance.SetLogIndexTip(AppEnums.LOG_INDEX_TIP.LOGIN).Error($"Ip adresi alnamadı.Bu nedenle login olunamaz.getLoginWithoutKisiByIdmDto metodu");
        //            throw new AppException(MessageCode.ERROR_500, "Ip adresi alnamadı.Bu nedenle login olunamaz");
        //        }

        //        repoKisiToken.Save(kt);
        //    }



        //    //UserDTO userDto = _mapper.Map<UserDTO>(kisi); ;

        //    UserTokenDTO ktDto = new UserTokenDTO();
        //    ktDto.ip = kt.IP_ADRES;
        //    ktDto.isLogin = true;
        //    ktDto.idmToken = idmDto.Token;
        //    ktDto.oysToken = kt.OYS_TOKEN;
        //    ktDto.userTypes = new List<string>();
        //    ktDto.expireDate = Convert.ToDateTime(kt.EXP_DATE);
        //    ktDto.id = kt.ID;
        //    ktDto.loginName = idmDto.UserName;

        //    ktDto.userDto = userDto;

        //    loginResponseDTO.userTokenDto = ktDto;

        //    var ktDtoForSerilize = ktDto.Clone();
        //    if (ktDtoForSerilize.userDto != null)
        //    {
        //        //bu alan çok yer kaplıyor db de tutmamak gerek.
        //        ktDtoForSerilize.userDto.base64FotoUrl = "";
        //    }

        //    //kişi token jsonu oluştur ve değerleri yaz - başla
        //    string jsonString = JsonConvert.SerializeObject(ktDtoForSerilize);
        //    var repoKisiTokenClob = new GenericRepository<T_oys_KisiTokenClob>();
        //    T_oys_KisiTokenClob tokenClb = new T_oys_KisiTokenClob
        //    {
        //        KISI_TOKEN_ID = kt.ID,
        //        LOGIN_DTO_JSON = jsonString
        //    };

        //    repoKisiTokenClob.Save(tokenClb);

        //    kt.LOGIN_DTO_JSON_CLOB_ID = tokenClb.ID;
        //    repoKisiToken.Save(kt);

        //    //kişi token jsonu oluştur ve değerleri yaz - bitti

        //    return loginResponseDTO;

        //}
    }
}