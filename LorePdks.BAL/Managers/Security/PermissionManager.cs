//using AutoMapper;
//using LorePdks.BAL.Managers.Security.Interfaces;
//using LorePdks.COMMON.DTO.Security.Permission;
//using LorePdks.DAL.Model;
//using LorePdks.DAL.Repository;
//using System.Globalization;
//using System.Text;


//namespace LorePdks.BAL.Managers.Security
//{
//    public class PermissionManager : IPermissionManager
//    {
//        private readonly IMapper _mapper;
//        private readonly IRoleUserManager _roleUserManager;
//        private readonly IRoleWidgetManager _roleWidgetManager;

//        public PermissionManager(IMapper mapper,
//            IRoleUserManager roleUserManager,
//            IRoleWidgetManager roleWidgetManager
//            )
//        {
//            _mapper = mapper;
//            _roleUserManager = roleUserManager;
//            _roleWidgetManager = roleWidgetManager;
//        }
//        public void Check(List<PermissionDTO> permissionListDto)
//        {
//            // Yetki sayfası yüklendiğinde sistemdeki tüm actionlar parametre olarak gelir (permissionListDTO).
//            List<T_Pos_AUTH_PERMISSION> permissionList = _mapper.Map<List<T_Pos_AUTH_PERMISSION>>(permissionListDto);

//            // Parametre olarak gelen tüm actionlar T_Pos_AUTH_PERMISSION tablosuna eklenir.
//            foreach (var item in permissionList)
//            {
//                if (item.CONTROLLER != "Base")
//                {
//                    Add(item);
//                }
//            }
//            // T_Pos_AUTH_PERMISSION tablosundaki actionlar ile parametre olarak gelen sistem actionları karşılaştırılır.
//            // T_Pos_AUTH_PERMISSION tablosunda olup sistemde olmayan actionları t_auth permission tablosundan siler.
//            CheckDiferences(permissionList);

//        }
//        public List<PermissionDTO> GetList()
//        {
//            GenericRepository<T_Pos_AUTH_PERMISSION> _repoPermission = new GenericRepository<T_Pos_AUTH_PERMISSION>();
//            var permissionList = _repoPermission.GetList();
//            List<PermissionDTO> permissionListDto = _mapper.Map<List<PermissionDTO>>(permissionList);
//            permissionListDto = permissionListDto
//                                                .OrderBy(x => x.type)
//                                                .ThenBy(x => x.area)
//                                                .ThenBy(x => x.controller)
//                                                .ThenBy(x => x.action).ToList();
//            return permissionListDto;
//        }//T_Pos_AUTH_PERMISSION Tablosundaki Action Listesini Çeker.
//        public List<PermissionDTO> GetPermissionListByUserId(long userId)
//        {
//            string query = @"select p.* from 
//            Pos.AUTH_ROLE_USER ru 
//            inner join Pos.AUTH_ROLE_WIDGET rw on rw.ROLE_ID=ru.ROLE_ID  and rw.ISDELETED=0
//            inner join Pos.AUTH_WIDGET_PERMISSION wp on wp.WIDGET_ID=rw.WIDGET_ID and wp.ISDELETED=0
//            inner join Pos.AUTH_PERMISSION p on p.Id=wp.PERMISSION_ID and p.ISDELETED=0
//            where ru.USER_ID=@p1
//            and ru.ISDELETED=0";
//            GenericRepository<T_Pos_AUTH_PERMISSION> _repoPermission = new GenericRepository<T_Pos_AUTH_PERMISSION>();
//            List<T_Pos_AUTH_PERMISSION> userPermissionList = _repoPermission.Query<T_Pos_AUTH_PERMISSION>(query, new { p1 = userId }).ToList();

//            List<PermissionDTO> userPermissionListDto = _mapper.Map<List<PermissionDTO>>(userPermissionList);
//            return userPermissionListDto;
//        }//login olan kullanıcısının yetkili olduğu actionları çeker.

//        private void Add(T_Pos_AUTH_PERMISSION permission)
//        {
//            GenericRepository<T_Pos_AUTH_PERMISSION> _repoPermission = new GenericRepository<T_Pos_AUTH_PERMISSION>();
//            T_Pos_AUTH_PERMISSION hasPermission = _repoPermission.Get(
//                "Type=@Type and Area=@Area and Controller=@Controller and Action=@Action and ReturnType=@ReturnType",
//                new
//                {
//                    Type = permission.TYPE,
//                    Area = permission.AREA,
//                    Controller = permission.CONTROLLER,
//                    Action = permission.ACTION,
//                    ReturnType = permission.RETURN_TYPE
//                });

//            if (hasPermission == null)
//            {
//                permission.NAME = $@"{permission.CONTROLLER.ToLower(new CultureInfo("en-US", true))}-{permission.ACTION.ToLower(new CultureInfo("en-US", true))}";
//                permission.URL = $@"/api/{permission.AREA.ToLower(new CultureInfo("en-US", true))}/{permission.CONTROLLER.ToLower(new CultureInfo("en-US", true))}/{permission.ACTION.ToLower(new CultureInfo("en-US", true))}";
//                _repoPermission.Insert(permission);
//            }
//        }//T_Pos_AUTH_PERMISSION Tablosuna Action'ı Ekler.
//        private void Del(long permissionsId)
//        {
//            GenericRepository<T_Pos_AUTH_WIDGET_PERMISSION> _repoWidgetPermission = new GenericRepository<T_Pos_AUTH_WIDGET_PERMISSION>();
//            List<T_Pos_AUTH_WIDGET_PERMISSION> widgetPermissionList = _repoWidgetPermission.GetList("PermissionId=@p1", new { p1 = permissionsId });
//            foreach (var widgetPermission in widgetPermissionList)
//            {
//                _repoWidgetPermission.Delete(widgetPermission);
//            }
//            GenericRepository<T_Pos_AUTH_PERMISSION> _repoPermission = new GenericRepository<T_Pos_AUTH_PERMISSION>();
//            T_Pos_AUTH_PERMISSION _permission = _repoPermission.Get(permissionsId);
//            _repoPermission.Delete(_permission);
//        }//T_Pos_AUTH_PERMISSION Tablosundan Action Siler.
//        private void CheckDiferences(List<T_Pos_AUTH_PERMISSION> systemPermissionList)
//        {
//            StringBuilder x = new StringBuilder();
//            StringBuilder y = new StringBuilder();
//            bool z = false;
//            //T_Pos_AUTH_PERMISSION Tablosundaki Tüm Action'ları Çeker.
//            List<PermissionDTO> permissionListDTO = GetList();
//            List<T_Pos_AUTH_PERMISSION> permissionList = _mapper.Map<List<T_Pos_AUTH_PERMISSION>>(permissionListDTO);

//            foreach (var permission in permissionList)
//            {
//                // T_Pos_AUTH_PERMISSION Tablosundaki Seçili Action'ı x'e Atar.
//                x.Append(permission.TYPE.ToLower(new CultureInfo("en-US", true)));
//                x.Append("-");
//                x.Append(permission.AREA.ToLower(new CultureInfo("en-US", true)));
//                x.Append("-");
//                x.Append(permission.CONTROLLER.ToLower(new CultureInfo("en-US", true)));
//                x.Append("-");
//                x.Append(permission.ACTION.ToLower(new CultureInfo("en-US", true)));
//                x.Append("-");
//                x.Append(permission.RETURN_TYPE.ToLower(new CultureInfo("en-US", true)));
//                foreach (var systemPermission in systemPermissionList)
//                {
//                    // systemPermissionList'teki Seçili Action'ı y'e Atar.
//                    y.Append(permission.TYPE.ToLower(new CultureInfo("en-US", true)));
//                    y.Append("-");
//                    y.Append(permission.AREA.ToLower(new CultureInfo("en-US", true)));
//                    y.Append("-");
//                    y.Append(permission.CONTROLLER.ToLower(new CultureInfo("en-US", true)));
//                    y.Append("-");
//                    y.Append(permission.ACTION.ToLower(new CultureInfo("en-US", true)));
//                    y.Append("-");
//                    y.Append(permission.RETURN_TYPE.ToLower(new CultureInfo("en-US", true)));
//                    if (x.ToString() == y.ToString())
//                    {
//                        //x==y ise T_Pos_AUTH_PERMISSION Tablosundaki Action systemPermissionList'de Vardır. z=true olur.
//                        z = true;
//                    }
//                    y = new StringBuilder();
//                }

//                if (z == false)
//                {
//                    //z <> true ise T_Pos_AUTH_PERMISSION Tablosundaki Action systemPermissionList'de Yoktur ve T_Pos_AUTH_PERMISSION Tablosundan Silinir
//                    Del(permission.ID);
//                }

//                z = false;
//                x = new StringBuilder();

//            }
//        }

//    }
//}
