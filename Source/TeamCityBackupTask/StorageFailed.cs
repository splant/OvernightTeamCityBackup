using System;

namespace TeamCityBackupTask
{
    public class StorageFailed : Exception
    {
        public StorageFailed(string failureMessage) : base(failureMessage) {}
    }
}