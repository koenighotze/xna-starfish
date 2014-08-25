using System;
using System.Collections.Generic;
using System.Text;
using Star.Util.Debug;


namespace Star.Util.Log
{
    class Logger : ILogger
    {

        public enum LEVEL
        {
            DEBUG,
            INFO,
            WARN,
            ERROR
        }

        private readonly String name;

        public Logger(String aName)
        {
            Assert.FailIfNull(aName, "aName");
            this.name = aName;
        }

        // TODO: replace with constructor lookup
        public Logger(Type aType) : this(aType.Name)
        {
        }

        public Boolean IsLevelEnabled(LEVEL aLevel)
        {
            return true;
        }

        public void Log(LEVEL aLevel, String aMessage)
        {
            Assert.FailIfNull(aLevel, "aLevel");
            Assert.FailIfNull(aMessage, "aMessage");

            if (IsLevelEnabled(aLevel))
            {
                //System.DateTime.Now.ToShortTimeString
#if !XBOX360
                Console.WriteLine("{0} {1}: {2}", DateTime.Now.ToString("HH:mm:ss"), aLevel, aMessage);
#endif 
            }
        }

        public void Debug(String format, params Object[] values)
        {
            Assert.FailIfNull(format, "format");
            Assert.FailIfNull(values, "values");
            Debug(String.Format(format, values));
        }

        public void Info(String format, params Object[] values)
        {
            Assert.FailIfNull(format, "format");
            Assert.FailIfNull(values, "values");
            Debug(String.Format(format, values));
        }

        public void Warn(String format, params Object[] values)
        {
            Assert.FailIfNull(format, "format");
            Assert.FailIfNull(values, "values");
            Warn(String.Format(format, values));

        }

        public void Error(String format, params Object[] values)
        {
            Assert.FailIfNull(format, "format");
            Assert.FailIfNull(values, "values");
            Error(String.Format(format, values));
        }


        public void Debug(String message)
        {
            Log(LEVEL.DEBUG, message);
        }

        public void Info(String message)
        {
            Log(LEVEL.INFO, message);
        }

        public void Warn(String message)
        {
            Log(LEVEL.WARN, message);
        }

        public void Error(String message)
        {
            Log(LEVEL.ERROR, message);
        }
    }
}
