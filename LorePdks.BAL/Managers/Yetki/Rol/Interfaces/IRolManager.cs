using LorePdks.COMMON.DTO.Yetki;
using LorePdks.COMMON.DTO.Yetki.Rol;

namespace LorePdks.BAL.Managers.Yetki.Rol.Interfaces
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
        
        // Controller-Method yetkilendirme için eklenen metodlar
        List<RolControllerMethodDTO> getRolControllerMethodDtoListByRolId(int rolId, bool isYoksaHataDondur = false);
        bool addControllerMethodToRol(int rolId, string controllerName, string methodName);
        bool removeControllerMethodFromRol(int rolId, string controllerName, string methodName);
        bool saveRolControllerMethods(int rolId, List<ControllerAndMethodsDTO> controllerMethods);
    }
}