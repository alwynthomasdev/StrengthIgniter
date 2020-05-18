using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Core.Utils
{
    public interface ILogUtility
    {
        void Log(LogType type, string message, Exception exception = null);
    }

    public enum LogType
    {
        Error,
        Warning,
        Info,
        Debug
    }
}
