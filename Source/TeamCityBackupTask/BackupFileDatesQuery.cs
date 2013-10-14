using System;
using System.Collections.Generic;

namespace TeamCityBackupTask
{
    public interface BackupFileDatesQuery 
    {
        IEnumerable<DateTime> GetDates(IEnumerable<string> backupFileNames);
    }
}