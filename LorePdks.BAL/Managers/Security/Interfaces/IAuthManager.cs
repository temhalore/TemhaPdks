using LorePdks.COMMON.DTO.Security.Auth;

namespace LorePdks.BAL.Managers.Security.Interfaces
{
    public interface IAuthManager
    {
        LoginResponseDTO LoginWithToken(LoginWithSSORequestDTO loginReqDto);



    }
}
