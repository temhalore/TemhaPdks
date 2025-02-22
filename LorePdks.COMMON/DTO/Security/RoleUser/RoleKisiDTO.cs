using System.Collections.Generic;
using LorePdks.COMMON.DTO.Security.Role;
using LorePdks.COMMON.DTO.Security.User;

namespace LorePdks.COMMON.DTO.Security.RoleUser
{
    public class RoleUserDTO
    {
        public RoleDTO roleDto { get; set; }
        public UserDTO userDto { get; set; }
        public List<UserDTO> userListDto { get; set; }
    }
}
