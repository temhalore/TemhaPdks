
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LorePdks.COMMON.DTO.Security.User;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.DTO.Common;

namespace LorePdks.COMMON.DTO.Security.Auth
{
    public class UserTokenDTO : BaseDTO
    {

        public string loginName { get; set; }
        public UserDTO userDto { get; set; }
        public bool isLogin { get; set; }
        public string loginAraciUygulamaToken { get; set; }
        public KodDTO loginAraciUygulamaTipKodDto { get; set; }
        public string appToken { get; set; }
        public string ip { get; set; }
        public List<string> userTypes { get; set; }

        public DateTime expireDate { get; set; }
        public string language { get; set; } = "tr";

        public bool isYerineLogin { get; set; }
        public string yerineLoginAdminLoginName { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime updatedate { get; set; }
        public string userAgent { get; set; }

        public string deleteReason { get; set; }

        //public List<UserActionPermissionDTO> actionPermissionDto { get; set; } -- volki bunu aç not:ae


    }

}
