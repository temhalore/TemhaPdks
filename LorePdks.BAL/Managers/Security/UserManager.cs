//using AutoMapper;
//using LorePdks.BAL.Managers.Security.Interfaces;
//using LorePdks.BAL.Services.DataTable.Interfaces;
//using LorePdks.COMMON.DTO.Security.User;
//using LorePdks.COMMON.DTO.Services.DataTable;
//using LorePdks.COMMON.Models;
//using LorePdks.DAL.Model;
//using LorePdks.DAL.Repository;

//namespace LorePdks.BAL.Managers.Security
//{
//    public class UserManager : IUserManager
//    {
//        private readonly IMapper _mapper;
//        //private readonly IGenericRepository<t_core_file> _repoFile;

//        private readonly IDataTableService _dataTableService;



//        public UserManager(IMapper mapper, IDataTableService dataTableService)

//        {
//            _mapper = mapper;
//            _dataTableService = dataTableService;


//        }
//        public DataTableResponseDTO<UserDTO> GetDataTableList(DataTableRequestDTO<UserDTO> request)
//        {
//            string query = @$"select u.* FROM T_Pos_AUTH_USER u where u.ISDELETED=0";
//            var dataTableResponse = _dataTableService.GetDataTableResponseByModelQuery<T_Pos_AUTH_USER, UserDTO>(query, request);

//            //foreach (var item in dataTableResponse.data)
//            //{
//            //    //item.nationalityCodeDto = _countryManager.GetCountry(item.nationalityCodeDto.id);
//            //    //item.registeredProvinceCodeDto = _provinceManager.GetProvince(item.registeredProvinceCodeDto.id);
//            //    //item.registeredDistrictCodeDto = _districtManager.GetDistrict(item.registeredDistrictCodeDto.id);
//            //    //item.genderCodeDto = _codeManager.Get(item.genderCodeDto);

//            //}
//            return dataTableResponse;
//        }

//        public List<UserDTO> GetUserList()
//        {
//            GenericRepository<T_Pos_AUTH_USER> _repoUser = new GenericRepository<T_Pos_AUTH_USER>();
//            List<T_Pos_AUTH_USER> userList = _repoUser.GetList();
//            List<UserDTO> userListDto = _mapper.Map<List<UserDTO>>(userList);
//            return userListDto;
//        }
//        public List<UserDTO> GetUserListBySearchValue(string request)
//        {
//            if (request.Length < 3)
//            {
//                return new List<UserDTO>();
//            }
//            string query = @"select *
//			FROM T_Pos_AUTH_USER
//			where 
//			(
//			IdentificationNumber like '%'+@searchValue+'%' or 
//			 Name+' '+LastName like '%'+@searchValue+'%' 
//			 ) and IsDeleted=0";
//            GenericRepository<T_Pos_AUTH_USER> _repoUser = new GenericRepository<T_Pos_AUTH_USER>();
//            List<T_Pos_AUTH_USER> userList = _repoUser.Query<T_Pos_AUTH_USER>(query, new { searchValue = request }).ToList();
//            List<UserDTO> userListDto = _mapper.Map<List<UserDTO>>(userList);

//            return userListDto;
//        }


//        public List<UserDTO> GetListById(List<long> userIdList)
//        {
//            GenericRepository<T_Pos_AUTH_USER> _repoUser = new GenericRepository<T_Pos_AUTH_USER>();
//            List<T_Pos_AUTH_USER> userList = _repoUser.GetList("Id in @p1", new { p1 = userIdList });
//            List<UserDTO> userListDto = _mapper.Map<List<UserDTO>>(userList);
//            return userListDto;
//        }
//        public UserDTO GetUserDtoByUserNameAndPassword(string userName, string password)
//        {
//            GenericRepository<T_Pos_AUTH_USER> _repoUser = new GenericRepository<T_Pos_AUTH_USER>();
//            T_Pos_AUTH_USER user = _repoUser.Get("UserName=@p1 and Password=@p2", new { p1 = userName, p2 = password });
//            if (user == null)
//            {
//                throw new AppException(500, "Kullanıcı adı veya parola hatalı");
//            }
//            UserDTO userDto = _mapper.Map<UserDTO>(user);
//            //userDto.languageCodeDto = _codeManager.Get(userDto.languageCodeDto);
//            //userDto.userImageUrl = _fileManager.GetFileUrl(LookupEnums.enumModule.organization, LookupEnums.enumFileType.userImage, user);
//            return userDto;
//        }

//        public UserDTO GetByToken(string token)
//        {
//            throw new NotImplementedException();
//        }

//        public long AddWithUserDto(UserDTO userDto)
//        {
//            GenericRepository<T_Pos_AUTH_USER> _repoUser = new GenericRepository<T_Pos_AUTH_USER>();
//            T_Pos_AUTH_USER hasUser = _repoUser.Get("UserName=@p1", new { p1 = userDto.loginName });
//            if (hasUser != null)
//            {
//                throw new AppException(500, "Bu Kullancı Adı ile daha önce kayıt yapılmış");
//            }
//            else
//            {
//                T_Pos_AUTH_USER user = _mapper.Map<T_Pos_AUTH_USER>(userDto);
//                long newUserId = _repoUser.Insert(user);
//                return newUserId;
//            }
//        }
//        public long Add(T_Pos_AUTH_USER user)
//        {
//            GenericRepository<T_Pos_AUTH_USER> _repoUser = new GenericRepository<T_Pos_AUTH_USER>();
//            T_Pos_AUTH_USER hasUser = _repoUser.Get("LOGIN_NAME=@p1", new { p1 = user.LOGIN_NAME });
//            if (hasUser != null)
//            {
//                throw new AppException(500, "Bu Kullanıcı Adı ile daha önce kayıt yapılmış");
//            }
//            else
//            {
//                long newUserId = _repoUser.Insert(user);
//                return newUserId;
//            }
//        }
//        public UserDTO Set(UserDTO userDto)
//        {
//            GenericRepository<T_Pos_AUTH_USER> _repoUser = new GenericRepository<T_Pos_AUTH_USER>();
//            T_Pos_AUTH_USER user = _repoUser.Get(userDto.id);
//            if (user != null)
//            {
//                user.AD = userDto.ad;
//                user.SOYAD = userDto.soyad;
//                //user.IdentificationNumber = userDto.identificationNumber;

//                //user.GenderCode = userDto.genderCodeDto.id;
//                //user.BirthDate = userDto.birthDate;
//                //user.BirthPlace = userDto.birthPlace;
//                //user.LanguageCode = userDto.languageCodeDto.id;
//                //user.FatherName = userDto.fatherName;
//                //user.MotherName = userDto.motherName;
//                //user.UserName = userDto.userName;

//                _repoUser.Update(user);
//                //if (userDto.userImageDto != null)
//                //{
//                //    //user.Photo = Encoding.ASCII.GetBytes(userDto.userImageDto.fileData);
//                //    //_fileManager.SaveFile<T_Pos_AUTH_USER>(userDto.userImageDto, user);
//                //    //TODO:Volkan
//                //}
//                return userDto;
//            }
//            else
//            {
//                throw new AppException(500, "Güncellenecek Kullanıcı bulunamadı");
//            }
//        }
//        public UserDTO GetById(long userId)
//        {
//            GenericRepository<T_Pos_AUTH_USER> _repoUser = new GenericRepository<T_Pos_AUTH_USER>();
//            T_Pos_AUTH_USER user = _repoUser.Get(userId);
//            UserDTO userDto = _mapper.Map<UserDTO>(user);
//            return userDto;
//        }
//        public UserDTO GetUserDtoByIdentificationNumber(string identificationNumber)
//        {
//            GenericRepository<T_Pos_AUTH_USER> _repoUser = new GenericRepository<T_Pos_AUTH_USER>();
//            T_Pos_AUTH_USER user = _repoUser.Get("IdentificationNumber=@p1", new { p1 = identificationNumber });
//            UserDTO userDto = _mapper.Map<UserDTO>(user);
//            return userDto;
//        }
//    }
//}

