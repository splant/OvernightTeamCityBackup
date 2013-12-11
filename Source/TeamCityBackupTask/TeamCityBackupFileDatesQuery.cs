using System;
using System.Collections.Generic;
using System.Globalization;

namespace TeamCityBackupTask
{
    public class TeamCityBackupFileDatesQuery : BackupFileDatesQuery 
    {
        public IEnumerable<BackupFileWithDateStamp> GetDates(IEnumerable<string> backupFileNames)
        {
            foreach (var backupFileName in backupFileNames)
            {
                var isoFormattedDate = GetIsoFormattedDate(backupFileName);

                DateTime backupFileDate;
                var isValidDateTime = DateTime.TryParseExact(isoFormattedDate, "yyyy-MM-dd'T'HH:mm:ss", 
                                                             CultureInfo.InvariantCulture, DateTimeStyles.None, out backupFileDate);

                if (isValidDateTime)
                    yield return new BackupFileWithDateStamp(backupFileName, backupFileDate);
            }
        }

        private string GetIsoFormattedDate(string backupFileName)
        {
            try
            {
                var dateStringComponentOnly = backupFileName.Replace("TeamCity_Backup_", "")
                                                            .Replace(".zip", "");

                var isoFormattedDate = dateStringComponentOnly.Replace("_", "T")
                                                              .Insert(4, "-")
                                                              .Insert(7, "-")
                                                              .Insert(13, ":")
                                                              .Insert(16, ":");
                return isoFormattedDate;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}