using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.DTO.Security.Auth
{
    public class LoginWithSSORequestDTO
    {
        public string ssoToken { get; set; }

        public long tip { get; set; }

        public long statu { get; set; }

        //süper tokenla kullanmak için
        public string loginName { get; set; }

    }
}
