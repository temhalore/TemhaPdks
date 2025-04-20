using LorePdks.COMMON.DTO.Auth.Securty.Rol;

namespace LorePdks.BAL.Managers.Auth.Yetki.Rol.Interfaces
{
    public interface IRolManager
    {
        List<RolDTO> getRolDtoList(bool isYoksaHataDondur = false);
        RolDTO getRolDtoById(int rolId, bool isYoksaHataDondur = false);
        List<RolDTO> getRolDtoListByKisiId(int kisiId, bool isYoksaHataDondur = false);
        RolDTO saveRol(RolDTO rolDTO);
        void deleteRolByRolId(int rolId);
        bool addEkranToRol(int rolId, int ekranId);
        bool removeEkranFromRol(int rolId, int ekranId);
        bool addRolToKisi(int kisiId, int rolId);
        bool removeRolFromKisi(int kisiId, int rolId);


    }
}