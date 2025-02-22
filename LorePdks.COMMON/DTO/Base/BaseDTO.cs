using LorePdks.COMMON.Helpers;
using System.Text.Json.Serialization;

namespace LorePdks.COMMON.DTO.Base
{
    public abstract class BaseDTO
    {
        string _eid;
        int _id;
        [JsonIgnore]
        public int id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                if (_id > 0)
                    _eid = CryptoHelper.EncryptString(_id.ToString());
            }
        }
        public string eid
        {
            get
            {
                return _eid;
            }
            set
            {
                _eid = value;
                if (!string.IsNullOrWhiteSpace(_eid))
                    _id = Convert.ToInt32(CryptoHelper.DecryptString(_eid));
            }

        }
        // public string refreshToken { get; set; }

        //public string responseMessage { get; set; } = null;
        //public string filterText { get; set; } = null;
        //public int pageNumber { get; set; } = 1;
        //public int itemsPerPage { get; set; } = 10;
        //public int totalItems { get; set; }

    }
}
