using LorePdks.COMMON.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LorePdks.DAL.Model
{
    public class _BaseModel
    {
        public bool IsNull()
        {
            var ID = (long)GetType().GetProperty(CoreConfig.IDProperty).GetValue(this);
            if (ID == 0)
                return true;

            return false;
        }

        public object Clone()
        {
            return (_BaseModel)MemberwiseClone();
        }
    }
}
