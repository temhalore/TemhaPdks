

using LorePdks.BAL.AutoMapper;
using LorePdks.COMMON.DTO.Common;
using LorePdks.DAL.Model;

public class PdksHareketMapper : MappingProfile
{
    public PdksHareketMapper()
    {
        CreateMap<t_pdks_hareket, PdksHareketDTO>()
            .ForMember(x => x.id, y => y.MapFrom(z => z.ID))
            .ForMember(x => x.pdksDto, y => y.MapFrom(z => new PdksDTO() { id = z.PDKS_ID }))
            .ForMember(x => x.hareketDto, y => y.MapFrom(z => new HareketDTO() { id = z.HAREKET_ID }))
            .ReverseMap()
            .ForMember(x => x.ID, y => y.MapFrom(z => z.id))
            .ForMember(x => x.PDKS_ID, y => y.MapFrom(z => z.pdksDto != null ? z.pdksDto.id : 0))
            .ForMember(x => x.HAREKET_ID, y => y.MapFrom(z => z.hareketDto != null ? z.hareketDto.id : 0));
    }
}
