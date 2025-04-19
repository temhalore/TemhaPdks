

using LorePdks.BAL.AutoMapper;
using LorePdks.COMMON.DTO.Common;
using LorePdks.DAL.Model;

public class HareketMappingProfile : MappingProfile
{
    public HareketMappingProfile()
    {
        CreateMap<HareketDTO, t_hareket>()
            .ForMember(dest => dest.FIRMA_ID, opt => opt.MapFrom(src => src.firmaDto.id))
            .ForMember(dest => dest.HAREKET_TIP_KID, opt => opt.MapFrom(src => src.hareketTipKodDto.id))
            .ForMember(dest => dest.HAREKET_DURUM_KID, opt => opt.MapFrom(src => src.hareketDurumKodDto.id))
            .ForMember(dest => dest.HAREKET_DATA, opt => opt.MapFrom(src => src.hareketdata))
            .ReverseMap()
            .ForMember(dest => dest.firmaDto, opt => opt.MapFrom(src => new FirmaDTO { id = src.FIRMA_ID }))
            .ForMember(dest => dest.hareketTipKodDto, opt => opt.MapFrom(src => _kodManager.Value.GetKodDtoByKodId(Convert.ToInt32(src.HAREKET_TIP_KID)) ))
            .ForMember(dest => dest.hareketDurumKodDto, opt => opt.MapFrom(src => _kodManager.Value.GetKodDtoByKodId(Convert.ToInt32(src.HAREKET_DURUM_KID)) ))
            .ForMember(dest => dest.hareketdata, opt => opt.MapFrom(src => src.HAREKET_DATA));
    }
}
