using System;

namespace TeamCityBackupTask
{
    public interface CurrentDateProvider
    {
        DateTime Now { get; }
    }
}