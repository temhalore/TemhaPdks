using LorePdks.COMMON.DTO.Base;

namespace LorePdks.COMMON.DTO.Security.User
{

    public class UserDTO : BaseDTO
    {
        public string loginName { get; set; }

        private string _tc;
        public string ad { get; set; }
        public string soyad { get; set; }




        public string tc
        {
            get
            {

                return $"*******{_tc?.Substring(_tc.Length - 4, 4)}";

            }
            set
            {
                _tc = value;
            }
        }
        public string tcx { get; set; }
        private int myVar;

        public int MyProperty
        {
            get { return myVar; }
        }

        public string fotoUrl { get; set; }
        public string base64FotoUrl { get; set; }



        public string adSoyad
        {
            get
            {
                return $"{ad} {soyad}";
            }

        }



    }
}
