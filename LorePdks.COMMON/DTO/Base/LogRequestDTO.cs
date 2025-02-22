
using System.Text.Json.Serialization;

namespace LorePdks.COMMON.DTO.Base
{
    public abstract class LogRequestDTO
    {
        public string eKisiId { get; set; }
        public string appToken { get; set; }
        public DateTime beginDatetime { get; set; }
        public DateTime endDatetime { get; set; }
        public int page { get; set; }
        public string levels { get; set; }
        public string patern { get; set; }
        public string mesaj { get; set; }
    }
}