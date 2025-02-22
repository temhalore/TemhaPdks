namespace LorePdks.COMMON.DTO.Security.Auth
{
    public class ForgotPasswordRequestDTO
    {
        //public CountryDTO nationalityCodeDto { get; set; }
        public string identificationNumber { get; set; }
        public DateTime birthDate { get; set; }
        public string name { get; set; }
        public string lastName { get; set; }
        public string phone { get; set; }
        //public string email{ get; set; }
        //public bool isInformationCorrect { get; set; }
    }
}
