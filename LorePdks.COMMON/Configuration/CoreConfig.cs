using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using vosetu.work.common.DTO.Security.Auth;

namespace LorePdks.COMMON.Configuration
{
    public class CoreConfig
    {
        public static string ConfigName { get; set; }
        //public  LoginResponseDTO LoginData { get; set; }
        public static string ConnectionString { get; set; }

        public static string IDProperty { get; set; }
        public static string CreatedDateProperty { get; set; }
        public static string CreatedUserProperty { get; set; }
        public static string ModifiedDateProperty { get; set; }
        public static string ModifiedUserProperty { get; set; }

        public static string CreatedIpAdressProperty { get; set; }
        public static string ModifiedIpAdressProperty { get; set; }

        public static string IsDeletedProperty { get; set; }
        public static string IdmApplicationCode { get; set; }
        public static string IdmApplicationPassword { get; set; }
        public static string IdmHostUrl { get; set; }
        public static string EbysServiceHostUrl { get; set; }
        public static string EbysServiceUsername { get; set; }
        public static string EbysServicePassword { get; set; }

        public static string TokenKeyName { get; set; }// "oysToken";

        public static string TokenCreateMin { get; set; } //"10";
        public static string TokenExpAddMin { get; set; } //"10";
        public static string TokenExpMin { get; set; } //"2";

        public static bool IsProd { get; set; }
        public static bool IsJobRun { get; set; }


        public static string UygulamaWebPath { get; set; }
        public static string UygulamaApiPath { get; set; }

        public static string FirmaDegerlendirmeUrl { get; set; }
        public static string AuzefCozumMerkeziHostUrl { get; set; }

        public static string AuzefCozumMerkeziApiToken { get; set; }

        public static string SSOUygulamaKod { get; set; }
        public static string SSOUygulamaParola { get; set; }

        public static string SSOHostUrl { get; set; }
        public static List<string> directAccessList { get; set; }

        public static string superToken { get; set; }




        //public  string DefaultUrl { get; set; }
        //public  string LoginUrl { get; set; }

        //public  string AuthCookieName { get; set; }
        //public  string ImageUrlPrefix { get; set; }
        //public  bool Production { get; set; }

        //public  string AuthhenticatorApplicationCode { get { return ConfigurationManager.AppSettings["AUTHENTICATOR_APLICATION_CODE"]; } }
        //public  string AuthhenticatorApplicationPassword { get { return ConfigurationManager.AppSettings["AUTHENTICATOR_PASSWORD"]; } }
        //public  string AuthhenticatorHostUrl { get { return ConfigurationManager.AppSettings["AUTHENTICATOR_HOST_URL"]; } }

        //public const string TokenCookieName = "appCookie";
        //public const string AuthHttpContextKeyName = "UserID";
        //public const string TokenKeyName = "Token";

    }
}
