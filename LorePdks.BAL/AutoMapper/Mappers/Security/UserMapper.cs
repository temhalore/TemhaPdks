//using AutoMapper;
//using kspys.common.DTO.Core;
//using kspys.common.DTO.Security.User;
//using kspys.dal.Model;
//using LorePdks.COMMON.DTO.Security.User;
//using System;

//namespace LorePdks.BAL.AutoMapper.Mappers.Security
//{
//    public class UserMapper : Profile
//    {
//        public UserMapper()
//        {
//            CreateMap<t_auth_user, UserDTO>()
//                   .ForMember(x => x.id, y => y.MapFrom(z => z.Id))
//                   .ForMember(x => x.identificationNumber, y => y.MapFrom(z => z.IdentificationNumber))
//                   .ForMember(x => x.name, y => y.MapFrom(z => z.Name))
//                   .ForMember(x => x.lastName, y => y.MapFrom(z => z.LastName))
//                   .ForMember(x => x.fatherName, y => y.MapFrom(z => z.FatherName))
//                   .ForMember(x => x.motherName, y => y.MapFrom(z => z.MotherName))
//                   .ForMember(x => x.birthDate, y => y.MapFrom(z => z.BirthDate))
//                   .ForMember(x => x.birthPlace, y => y.MapFrom(z => z.BirthPlace))
//                   //.ForMember(x => x.nationalityCodeDto, y => y.MapFrom(z => new CountryDTO() { code = Convert.ToInt64(z.NationalityCode) }))
//                   //.ForMember(x => x.registeredProvinceCodeDto, y => y.MapFrom(z => new ProvinceDTO() { id = Convert.ToInt64(z.RegisteredProvinceCode) }))
//                   //.ForMember(x => x.registeredDistrictCodeDto, y => y.MapFrom(z => new DistrictDTO() { id = Convert.ToInt64(z.RegisteredDistrictCode) }))
//                   .ForMember(x => x.volumeNo, y => y.MapFrom(z => z.VolumeNo))
//                   .ForMember(x => x.familyOrderNo, y => y.MapFrom(z => z.FamilyOrderNo))
//                   .ForMember(x => x.orderNo, y => y.MapFrom(z => z.OrderNo))
//                   .ForMember(x => x.userName, y => y.MapFrom(z => z.UserName))
//                   .ForMember(x => x.password, y => y.MapFrom(z => z.Password))
//                   .ForMember(x => x.genderCodeDto, y => y.MapFrom(z => new CodeDTO() { id = Convert.ToInt64(z.GenderCode) }))
//                   .ForMember(x => x.languageCodeDto, y => y.MapFrom(z => new CodeDTO() { id = Convert.ToInt64(z.LanguageCode) }))

//                   .ForMember(x => x.isInformationCorrect, y => y.MapFrom(z => z.IsInformationCorrect ))

//                   .ForMember(x => x.isTheApplicationFeeEstablished, y => y.MapFrom(z => z.IsTheApplicationFeeEstablished))
//                   .ForMember(x => x.isTheApplicationFeePaid, y => y.MapFrom(z => z.IsTheApplicationFeePaid))
//                   .ReverseMap()
//                   .ForMember(x => x.Id, y => y.MapFrom(z => z.id))
//                   .ForMember(x => x.IdentificationNumber, y => y.MapFrom(z => z.identificationNumber))
//                   .ForMember(x => x.Name, y => y.MapFrom(z => z.name))
//                   .ForMember(x => x.LastName, y => y.MapFrom(z => z.lastName))
//                   .ForMember(x => x.FatherName, y => y.MapFrom(z => z.fatherName))
//                   .ForMember(x => x.MotherName, y => y.MapFrom(z => z.motherName))
//                   .ForMember(x => x.BirthDate, y => y.MapFrom(z => z.birthDate))
//                   .ForMember(x => x.BirthPlace, y => y.MapFrom(z => z.birthPlace))
//                   .ForMember(x => x.NationalityCode, y => y.MapFrom(z => z.nationalityCodeDto.code))
//                   .ForMember(x => x.RegisteredProvinceCode, y => y.MapFrom(z => z.registeredProvinceCodeDto.code))
//                   .ForMember(x => x.RegisteredDistrictCode, y => y.MapFrom(z => z.registeredDistrictCodeDto.code))
//                   .ForMember(x => x.VolumeNo, y => y.MapFrom(z => z.volumeNo))
//                   .ForMember(x => x.FamilyOrderNo, y => y.MapFrom(z => z.familyOrderNo))
//                   .ForMember(x => x.OrderNo, y => y.MapFrom(z => z.orderNo))
//                   .ForMember(x => x.UserName, y => y.MapFrom(z => z.userName))
//                   .ForMember(x => x.Password, y => y.MapFrom(z => z.password))
//                   .ForMember(x => x.GenderCode, y => y.MapFrom(z => z.genderCodeDto.id))
//                   .ForMember(x => x.LanguageCode, y => y.MapFrom(z => z.languageCodeDto.id))
//                   .ForMember(x => x.IsInformationCorrect, y => y.MapFrom(z => z.isInformationCorrect))
//                   .ForMember(x => x.IsTheApplicationFeeEstablished, y => y.MapFrom(z => z.isTheApplicationFeeEstablished))
//                   .ForMember(x => x.IsTheApplicationFeePaid, y => y.MapFrom(z => z.isTheApplicationFeePaid))
//                   ;
//        }
//    }
//}
