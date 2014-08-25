using System;
using System.Collections.Generic;
using System.Text;

namespace Star.Util.Log
{
    public class ILoggerFactory
    {
        public static ILogger getLogger(String aName)
        {
            return new Logger(aName);
        }

        public static ILogger getLogger(Type aType)
        {
            return new Logger(aType);
        }
    }
}
