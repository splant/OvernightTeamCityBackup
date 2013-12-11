using System;
using System.Collections.Generic;

namespace TeamCityBackupTask
{
    public interface BackupFileDatesQuery 
    {
        IEnumerable<BackupFileWithDateStamp> GetDates(IEnumerable<string> backupFileNames);
    }

    public class BackupFileWithDateStamp
    {
        public BackupFileWithDateStamp(string backupFileName, DateTime backupDateTime)
        {
            BackupFileName = backupFileName;
            BackupDateTime = backupDateTime;
        }

        public string BackupFileName { get; private set; }
        public DateTime BackupDateTime { get; private set; }
    }
}