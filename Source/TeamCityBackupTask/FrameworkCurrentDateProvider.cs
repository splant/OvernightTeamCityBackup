using System;

namespace TeamCityBackupTask
{
    public class FrameworkCurrentDateProvider : CurrentDateProvider 
    {
        public DateTime Now { get { return DateTime.Now; } }
    }
}