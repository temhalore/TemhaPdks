using AutoMapper;
using LorePdks.BAL.Managers.Common.Kod.Interfaces;
using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Extensions;
using LorePdks.COMMON.Models;
using Microsoft.AspNetCore.Http;
using Nest;
using LorePdks.DAL.Model;
using LorePdks.DAL.Repository;
using System.Net;


namespace LorePdks.BAL.Managers.Common.Kod
{
    public class KodManager : IKodManager
    {

        public static List<KodDTO> _allKodDtoList = new List<KodDTO>();
        public List<KodDTO> getAllKodDtoList { get { return allKodDtoList(); } }

        private IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public KodManager(IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            //_modelDtoConverterHelper = modelDtoConverterHelper;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;


        }

        /// <summary>
        /// cacheYenilensinMi = true giderse db den tüm kod alanlarını tekrardan listeye yazar
        /// </summary>
        /// <param name="cacheYenilensinMi"></param>
        /// <returns></returns>
        public List<KodDTO> allKodDtoList(bool cacheYenilensinMi = false)
        {

            if (_allKodDtoList.Count == 0 || _allKodDtoList == null)
            {
                cacheYenilensinMi = true;
            }

            if (cacheYenilensinMi)
            {
                var repo = new GenericRepository<T_Pos_Kod>();

                var data = repo.GetList();

                foreach (var item in data)
                {


                    KodDTO dto = new KodDTO();
                    try
                    {
                        dto = _mapper.Map<T_Pos_Kod, KodDTO>(item);
                    }
                    catch (Exception e)
                    {

                        dto.id = item.ID;
                        dto.kod = item.KOD;
                        dto.sira = Convert.ToInt32(item.SIRA);
                        dto.tipId = Convert.ToInt32(item.TIP_ID);
                        dto.kisaAd = item.KISA_AD;
                        dto.digerUygEnumDeger = Convert.ToInt32(item.DIGER_UYG_ID);
                        dto.digerUygEnumAd = item.DIGER_UYG_ENUM_AD;
                    }

                    //yoksa ekle
                    if (!_allKodDtoList.Select(x => x.id).ToList().Contains(dto.id)) _allKodDtoList.Add(dto);
                }

                //_allKodDtoList = Mapper.Map<List<T_OSS_KOD>, List<KodDTO>>(data);

            }

            return _allKodDtoList;
        }


        public bool checkKodDTOIdInTipList(KodDTO kodDto, AppEnums.KodTipList enumTip, string hataMesaji = "")
        {

            bool returnDeger = checkKodDTOIsNull(kodDto, hataMesaji);

            string mesaj = "Seçilen bir alan kendi listesinin dışında gelen Id: " + kodDto.id;
            if (!string.IsNullOrWhiteSpace(hataMesaji) && !string.IsNullOrEmpty(hataMesaji))
            {
                mesaj = hataMesaji;
            }

            if (!getAllKodDtoList.Where(x => x.tipId == (int)enumTip).Select(x => x.id).ToList().Contains(kodDto.id))
            {
                returnDeger = false;
            }

            if (!returnDeger)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, mesaj);
            }

            return returnDeger;
        }

        public bool checkKodDTOIsNull(KodDTO kodDto, string hataMesaji = "")
        {
            {
                bool returnDeger = false;
                if (kodDto != null && kodDto.id > 0)
                {
                    //kod listler arısnnda gelen id varmı buda tutarlılık için önemli
                    if (getAllKodDtoList.Select(x => x.id).ToList().Contains(kodDto.id))
                    {
                        returnDeger = true;
                    }
                }

                string mesaj = "Seçilmesi zorunlu bir alan seçilmemiş.";
                if (!string.IsNullOrWhiteSpace(hataMesaji) && !string.IsNullOrEmpty(hataMesaji))
                {
                    mesaj = hataMesaji;
                }

                if (!returnDeger)
                {
                    throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, mesaj);
                }

                return returnDeger;
            }
        }

        public KodDTO GetKodDtoByKodId(int kodId)
        {
            var repoKod = new GenericRepository<T_Pos_Kod>();
            var kod = repoKod.Get(kodId);
            var dto = _mapper.Map<T_Pos_Kod, KodDTO>(kod);
            return dto;
        }

        public List<KodDTO> GetKodDtoListByKodTipId(int kodTipID)
        {
            var dtoList = getAllKodDtoList.Where(x => x.tipId == kodTipID).ToList();
            var clone = dtoList.Clone();
            return clone;
        }

        //public KodDTO GetKodByDigerUygulamaEnumNameAndEnumDeger(AppEnums.DIGER_UYG_ENUM_NAME digerUygEnumName, int? digerUygEnumDeger)
        //{
        //    if (digerUygEnumDeger == null)
        //    {
        //        digerUygEnumDeger = -111;
        //    }
        //    var dto = getAllKodDtoList.Where(x => x.digerUygEnumAd == digerUygEnumName.ToString() && x.digerUygEnumDeger == Convert.ToInt64(digerUygEnumDeger)).FirstOrDefault();
        //    var clone = GeneralExtensions.Clone<KodDTO>(dto);
        //    return clone;

        //}

        public string refreshKodListCache()
        {
            allKodDtoList(true);
            return "KodDtoList yenilendi";
        }


    }
}
