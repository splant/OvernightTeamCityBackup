using System;

namespace TeamCityBackupTask
{
    public class BackupFailed : Exception 
    {
        public BackupFailed(string failureMessage) : base(failureMessage) {}
    }
}