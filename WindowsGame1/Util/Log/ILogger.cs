using System;
using System.Collections.Generic;

namespace Star.Util.Log
{
    public interface ILogger
    {
        void Debug(String format, params Object[] values);
        void Debug(String message);

        void Info(String format, params Object[] values);
        void Info(String message);

        void Warn(String format, params Object[] values);
        void Warn(String message);

        void Error(String format, params Object[] values);
        void Error(String message);
    }
}
