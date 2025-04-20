using System;
using System.ComponentModel.DataAnnotations;

namespace LorePdks.COMMON.DTO.Common
{
    public class KisiTokenDTO
    {
        public int id { get; set; }
        public int kisiId { get; set; }
        public string loginName { get; set; }
        public string token { get; set; }
        public string ipAdresi { get; set; }
        public string userAgent { get; set; }
        public DateTime? expDate { get; set; }
        public bool isLogin { get; set; }
        public KisiDTO kisiDto { get; set; }
    }
}