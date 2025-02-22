using LorePdks.COMMON.DTO.Security.Auth;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Enums;

namespace LorePdks.BAL.Managers.Security.Interfaces
{
    public interface IKisiTokenManager
    {


        LoginResponseDTO getLoginBySSODto(LoginWithSSOResponseDTO ssoEntity);

        UserTokenDTO GetKisiTokenDtoByToken(string token);

        void Logout(string token);

        void DeleteKisiTokenByToken(string token, AppEnums.KISI_TOKEN_DELETE_REASON deleteSebepKod, string deleteSebepAciklama = "");

        void DeleteKisiTokenByLoginName(string loginName, AppEnums.KISI_TOKEN_DELETE_REASON deleteSebepKod, string deleteSebepAciklama = "");

        UserTokenDTO tokenValidate(string token);



    }
}
