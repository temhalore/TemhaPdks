using AutoMapper;
using LorePdks.BAL.Managers.Helper.Interfaces;
using LorePdks.BAL.Managers.Security.Interfaces;
using LorePdks.COMMON.Aspects.Logging.Serilog;
using LorePdks.COMMON.Configuration;
using LorePdks.COMMON.DTO.Security.Auth;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Helpers;
using LorePdks.COMMON.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using LorePdks.COMMON.Enums;

namespace LorePdks.BAL.Managers.Security
{
    public class AuthManager : IAuthManager
    {
        // private IMapper _mapper;
        private IKisiTokenManager _kisiTokenManager;
        private IHelperManager _helperManager;
        //volki burayı aç
        //private IMenuManager _menuManager;
        //private readonly IPersmissionManager _persmissionManager;
        private Lazy<IAuthManager> _authManager;
        private readonly ILogger<AuthManager> _logger;

        // private IModelDtoConverterHelper _modelDtoConverterHelper;
        public AuthManager(Lazy<IAuthManager> authManeger,
            IMapper mapper,
            IKisiTokenManager kisiTokenManager,
            IHelperManager helperManager,
            ILogger<AuthManager> logger
            //IMenuManager menuManager, 
            //IPersmissionManager persmissionManager
            )
        {
            // _mapper = mapper;
            _kisiTokenManager = kisiTokenManager;
            _helperManager = helperManager;
            _logger = logger;
            // _menuManager = menuManager;
            //this._persmissionManager = persmissionManager;
            _authManager = authManeger;

        }
        /// <summary>
        /// sso üzerinden yönlendirme ile token ve ek bilgiler gelebilir 
        /// token kısmına ayrıca süper token atıp işlem geçilmesi yapılabilir
        /// </summary>
        /// <param name="loginReqDto"></param>
        /// <returns></returns>
        /// <exception cref="AppException"></exception>
        public LoginResponseDTO LoginWithToken(LoginWithSSORequestDTO loginReqDto)
        {
            LoginResponseDTO retunObj = new LoginResponseDTO();

            if (loginReqDto.ssoToken.Length > 4 && loginReqDto.ssoToken.Substring(0, 5) == "ssoiu")
            {
                var req = new LoginWithSSORequestDTO();
                req.ssoToken = loginReqDto.ssoToken;

                LoginResponseDTO ssoRes = LoginWithSSO(req);

                retunObj = ssoRes;

                if (ssoRes.userTokenDto.isLogin)
                {
                    // _logger.LogInformation("{loginReqLog}" + $"SSOtoken:{loginReqDto.token} aksisten SSO token  geldi, {loginReqDto.loginName} login başarılı dönüldü");
                    AppLogService.Instance.SetLogIndexTip(AppEnums.LOG_INDEX_TIP.LOGIN).Info($"SSOtoken:{loginReqDto.ssoToken} SSO token  geldi, {loginReqDto.loginName} login başarılı dönüldü");

                }
                else
                {
                    retunObj.userTokenDto.isLogin = false;
                    AppLogService.Instance.SetLogIndexTip(AppEnums.LOG_INDEX_TIP.LOGIN).Error($"SSOtoken:{loginReqDto.ssoToken} SSO token geldi ama hata var, {loginReqDto.loginName} login başarılı olamadı");

                }

            }
            else
            {

                LoginWithSSOResponseDTO ssoTokenResponseDto = new LoginWithSSOResponseDTO();
                //idm siz giriş için
                if (loginReqDto.ssoToken == CoreConfig.superToken)
                {
                    if (string.IsNullOrEmpty(loginReqDto.ssoToken))
                    {
                        throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, "loginName bilgisi gelmedi");
                    }

                    ssoTokenResponseDto.isLogin = true;
                    ssoTokenResponseDto.loginName = loginReqDto.loginName;
                    ssoTokenResponseDto.ssoToken = "süper token ile logini olundu : " + loginReqDto.loginName + " için";

                    AppLogService.Instance.SetLogIndexTip(AppEnums.LOG_INDEX_TIP.LOGIN).Info("süper token ile login olundu");
                }

                if (ssoTokenResponseDto.isLogin == true)
                {

                    retunObj = _kisiTokenManager.getLoginBySSODto(ssoTokenResponseDto);

                    AppLogService.Instance.SetLogIndexTip(AppEnums.LOG_INDEX_TIP.LOGIN).Info($"token:{loginReqDto.ssoToken} sso token geldi, login başarılı, {retunObj.userTokenDto} dönüldü");
                }
                else
                {
                    retunObj.userTokenDto.isLogin = false;
                    AppLogService.Instance.SetLogIndexTip(AppEnums.LOG_INDEX_TIP.LOGIN).Error($"token:{loginReqDto.ssoToken}  sso token geldi, login başarılı olamadı.IDMErr:" + ssoTokenResponseDto.errorMessage);
                }
            }

            return retunObj;
        }



        /// <summary>
        ///  ilk aksisten gelinirken düşülen metod
        /// </summary>
        /// <param name="idmToken"></param>
        /// <returns></returns>
        public LoginResponseDTO LoginWithTokenYerineLogin(LoginWithTokenYerineLoginRequestDTO loginReqDto)
        {
            var postData = JsonConvert.SerializeObject(new
            {
                uygulamaKod = CoreConfig.SSOUygulamaKod,
                uygulamaParola = CoreConfig.SSOUygulamaParola,
                loginReqDto.ssoToken,
            });

            string ssoResponse = HttpHelper.Post(CoreConfig.SSOHostUrl + "/Api/Auth/GetLoginTokenInfo", new Dictionary<string, string>() { }, postData);
            var ssoEntity = JsonConvert.DeserializeObject<LoginWithSSOResponseDTO>(ssoResponse);


            LoginResponseDTO retunObj = new LoginResponseDTO();


            if (ssoEntity.isLogin == true)
            {
                // idm okeyse artık yerine login olunacak kişi için işlemnler yapılsın aslında yerine login olunacak kişi için login açacağız


                ssoEntity.loginName = loginReqDto.yerineLoginName;
                ssoEntity.ssoToken = "Yerine Login olan kişi : " + ssoEntity.loginName + " dir";
                ssoEntity.isYerineLogin = true;
                ssoEntity.yerineLoginOlanAdminUserName = loginReqDto.loginName;
                ssoEntity.yerineLoginOlanAdminAppToken = loginReqDto.appToken; // bizim kişi tokenımız ama birinin yerine geçen kişinin sistemdeki token ını temsil eder
                retunObj = _kisiTokenManager.getLoginBySSODto(ssoEntity);


                //retunObj.menuListDto = _menuManager.GetMenuListByUserId((int)OysEnums.YETKI_APP_LIST.OGRENCI, retunObj.userDto.id);
                //retunObj.PageListDto = _menuManager.GetPageListByUserId((int)OysEnums.YETKI_APP_LIST.OGRENCI, retunObj.userDto.id);

                AppLogService.Instance.SetLogIndexTip(AppEnums.LOG_INDEX_TIP.LOGIN).Info($"token:{loginReqDto.appToken} kişi tokenlı kişi {loginReqDto.yerineLoginName} login nameli kişi için yerine login için kullanıldı login başarılı, retunObj.userTokenDto:{retunObj.userTokenDto}");

                // _logger.LogInformation("{loginReqLog}" + $"token:{loginReqDto.oysToken} oys-adminden geldi {loginReqDto.yerineLoginName} login nameli kişi için yerine login için kullanıldı, login başarılı, {retunObj.userTokenDto} dönüldü");

            }
            else
            {
                retunObj.userTokenDto.isLogin = false;

            }


            return retunObj;
        }


        private LoginResponseDTO LoginWithSSO(LoginWithSSORequestDTO req)
        {
            var postData = JsonConvert.SerializeObject(new
            {
                uygulamaKod = CoreConfig.SSOUygulamaKod,
                uygulamaParola = CoreConfig.SSOUygulamaParola,
                req.ssoToken,
            });

            string ssoResponse = HttpHelper.Post(CoreConfig.SSOHostUrl + "/Api/Auth/GetLoginTokenInfo", new Dictionary<string, string>() { }, postData);
            var ssoEntity = JsonConvert.DeserializeObject<LoginWithSSOResponseDTO>(ssoResponse);

            LoginResponseDTO retunObj = new LoginResponseDTO();

            if (ssoEntity.isLogin == true)
            {
                retunObj = _kisiTokenManager.getLoginBySSODto(ssoEntity);
                AppLogService.Instance.SetLogIndexTip(AppEnums.LOG_INDEX_TIP.LOGIN).Info($"token:{req.ssoToken} sso token ile kişi token oluşturuldu alınan kişi token:{retunObj.userTokenDto.appToken} ");
            }
            else
            {
                retunObj.userTokenDto.isLogin = false;
                AppLogService.Instance.SetLogIndexTip(AppEnums.LOG_INDEX_TIP.LOGIN).Error($"token:{req.ssoToken} sso token ile kişi token oluşurken ahta aldı.sso Hata:{ssoEntity.errorMessage}");
                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, ssoEntity.errorMessage);
            }

            return retunObj;
        }

    }
}