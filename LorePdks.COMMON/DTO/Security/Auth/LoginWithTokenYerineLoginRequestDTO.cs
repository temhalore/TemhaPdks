using LorePdks.COMMON.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.DTO.Security.Auth
{
    public class LoginWithTokenYerineLoginRequestDTO : BaseDTO
    {
        public string appToken { get; set; } // admin oys token
        public string ssoToken { get; set; } // admin sso token
        public string loginName { get; set; } // admin loginname token
        public string yerineLoginName { get; set; } // yerine login(öğrenci) olunacak kişinin login name i

    }
}
