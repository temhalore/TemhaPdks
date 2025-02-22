using System;

namespace LorePdks.COMMON.Attributes
{
    public class PermissionAttribute : Attribute
    {
        public string TypeName { get; set; }
        public PermissionAttribute(string _type)
        {
            TypeName = _type;

        }
    }
    public class PermissionReadAttribute : PermissionAttribute
    {
        public PermissionReadAttribute() : base("read")
        {
        }
    }
    public class PermissionWriteAttribute : PermissionAttribute
    {
        public PermissionWriteAttribute() : base("write")
        {
        }
    }
    public class PermissionDeleteAttribute : PermissionAttribute
    {
        public PermissionDeleteAttribute() : base("delete")
        {
        }
    }

}
