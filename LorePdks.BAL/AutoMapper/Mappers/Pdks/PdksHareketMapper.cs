

using LorePdks.BAL.AutoMapper;
using LorePdks.COMMON.DTO.Hareket;
using LorePdks.COMMON.DTO.Pdks;
using LorePdks.DAL.Model;

public class PdksHareketMapper : MappingProfile
{
    public PdksHareketMapper()
    {
        CreateMap<t_pdks_hareket, PdksHareketDTO>()
            .ForMember(x => x.id, y => y.MapFrom(z => z.ID))
            .ReverseMap()
            .ForMember(x => x.ID, y => y.MapFrom(z => z.id));
    }
}
