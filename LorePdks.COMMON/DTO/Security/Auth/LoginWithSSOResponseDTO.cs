using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.DTO.Security.Auth
{
    public class LoginWithSSOResponseDTO
    {
        public string ssoToken { get; set; }
        public long loginAppTipId { get; set; }
        public string loginAppTip { get; set; }
        public string loginAppToken { get; set; }
        public long tokenStatuId { get; set; }
        public string tokenStatu { get; set; }
        public string tokenUygulamaKod { get; set; }
        public string loginName { get; set; }
        public string adSoyad { get; set; }
        public string kimlikNo { get; set; }
        public string userAgent { get; set; }
        public string ip { get; set; }
        public string serverInfo { get; set; }
        public bool isLogin { get; set; }
        public string tokenAciklama { get; set; }
        public bool isError { get; set; }
        public string errorMessage { get; set; }


        ///yerinelogin için idm de bunlar yok
        public bool isYerineLogin { get; set; }
        public string yerineLoginOlanAdminUserName { get; set; }
        public string yerineLoginOlanAdminAppToken { get; set; }


    }
}
