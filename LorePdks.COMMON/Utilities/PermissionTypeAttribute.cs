using System;

namespace LorePdks.COMMON.Utilities
{
    public class PermissionTypeAttribute : Attribute
    {
        public string TypeName { get; set; }
        public PermissionTypeAttribute(string _type)
        {
            TypeName = _type;

        }
    }
    public class PermissionReadAttribute : PermissionTypeAttribute
    {
        public PermissionReadAttribute() : base("read")
        {
        }
    }
    public class PermissionWriteAttribute : PermissionTypeAttribute
    {
        public PermissionWriteAttribute() : base("write")
        {
        }
    }
    public class PermissionDeleteAttribute : PermissionTypeAttribute
    {
        public PermissionDeleteAttribute() : base("delete")
        {
        }
    }

}
