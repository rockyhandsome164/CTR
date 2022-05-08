using System;

namespace CTR_FLS_2.Services
{
    public interface ILoggingServices
    {
        void LogError(Exception TheBadStuff);
    }
}