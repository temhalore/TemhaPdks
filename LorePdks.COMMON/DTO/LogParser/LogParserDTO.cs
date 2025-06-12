using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.DTO.Firma;
using System.Collections.Generic;

namespace LorePdks.COMMON.DTO.LogParser
{
    /// <summary>
    /// Log Parser konfigürasyon DTO
    /// </summary>
    public class LogParserDTO : BaseDTO
    {
        public FirmaDTO firmaDto { get; set; }
        public string ad { get; set; }
        public string aciklama { get; set; }
        public string cihazTip { get; set; }
        
        // Parser Configuration
        public string delimiter { get; set; }
        public string dateFormat { get; set; }
        public string timeFormat { get; set; }
        public string regexPattern { get; set; }
        public string fieldMappingJson { get; set; }
        public string sampleLogData { get; set; }
        
        public bool aktif { get; set; }
    }
}
