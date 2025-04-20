

using LorePdks.BAL.AutoMapper;
using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.DTO.Firma;
using LorePdks.COMMON.DTO.FirmaCihaz;
using LorePdks.COMMON.DTO.Kisi;
using LorePdks.COMMON.DTO.Pdks;
using LorePdks.DAL.Model;

public class PdksMapper : MappingProfile
{
    public PdksMapper()
    {
        CreateMap<t_pdks, PdksDTO>()
            .ForMember(x => x.id, y => y.MapFrom(z => z.ID))
            .ForMember(x => x.girisTarih, y => y.MapFrom(z => z.GIRIS_TARIH))
            .ForMember(x => x.cikisTarih, y => y.MapFrom(z => z.CIKIS_TARIH != null ? DateTime.Parse(z.CIKIS_TARIH) : (DateTime?)null))
            .ForMember(x => x.firmaDto, y => y.MapFrom(z => new FirmaDTO() { id = z.FIRMA_ID }))
            .ForMember(x => x.kisiDto, y => y.MapFrom(z => new KisiDTO() { id = z.KISI_ID }))
            .ForMember(x => x.firmaCihazDto, y => y.MapFrom(z => new FirmaCihazDTO() { id = z.FIRMA_CIHAZ_ID }))
            .ReverseMap()
            .ForMember(x => x.ID, y => y.MapFrom(z => z.id))
            .ForMember(x => x.GIRIS_TARIH, y => y.MapFrom(z => z.girisTarih))
            .ForMember(x => x.CIKIS_TARIH, y => y.MapFrom(z => z.cikisTarih != null ? z.cikisTarih.ToString() : null))
            .ForMember(x => x.FIRMA_ID, y => y.MapFrom(z => z.firmaDto != null ? z.firmaDto.id : 0))
            .ForMember(x => x.KISI_ID, y => y.MapFrom(z => z.kisiDto != null ? z.kisiDto.id : 0))
            .ForMember(x => x.FIRMA_CIHAZ_ID, y => y.MapFrom(z => z.firmaCihazDto != null ? z.firmaCihazDto.id : 0));
    }
}
