using LorePdks.COMMON.Aspects.Logging.Serilog.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.Aspects.Logging
{
    public class LoggerFactory
    {
        private LoggerFactory()
        {

        }
        static readonly object _lock = new object();

        private static ExceptionLogger _exceptionLogger;
        public static ExceptionLogger ExceptionLogger
        {
            get
            {
                if (_exceptionLogger == null)//double check singleton
                {
                    lock (_lock)//thread safe singleton
                    {
                        if (_exceptionLogger == null)
                            _exceptionLogger = new ExceptionLogger();
                    }
                }
                return _exceptionLogger;
            }

        }

        private static UserActionLogger _userActionLogger;
        public static UserActionLogger UserActionLogger
        {
            get
            {
                if (_userActionLogger == null)//double check singleton
                {
                    lock (_lock)//thread safe singleton
                    {
                        if (_userActionLogger == null)
                            _userActionLogger = new UserActionLogger();
                    }
                }
                return _userActionLogger;
            }

        }
    }
}
