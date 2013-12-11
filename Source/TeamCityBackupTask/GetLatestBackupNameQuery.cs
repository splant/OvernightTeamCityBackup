using System.Collections.Generic;
using System.Linq;

namespace TeamCityBackupTask
{
    public class GetLatestBackupNameQuery : LatestRecentBackupQuery 
    {
        private readonly BackupSettings _backupSettings;
        private readonly FileSystem _fileSystem;
        private readonly BackupFileDatesQuery _backupFileDatesQuery;

        public GetLatestBackupNameQuery(
            BackupSettings backupSettings, FileSystem fileSystem, BackupFileDatesQuery backupFileDatesQuery)
        {
            _backupSettings = backupSettings;
            _fileSystem = fileSystem;
            _backupFileDatesQuery = backupFileDatesQuery;
        }

        public string GetName()
        {
            List<BackupFileWithDateStamp> backupFilesWithDateStamps = GetBackupFilesWithDateStamps();

            if (!backupFilesWithDateStamps.Any())
                return "";

            return GetLatestBackupFileName(backupFilesWithDateStamps);
        }

        private List<BackupFileWithDateStamp> GetBackupFilesWithDateStamps()
        {
            var backupFileNames = _fileSystem.GetFileNames(_backupSettings.BackupFilesLocation);
            return _backupFileDatesQuery.GetDates(backupFileNames).ToList();
        }

        private string GetLatestBackupFileName(List<BackupFileWithDateStamp> backupFilesWithDateStamps)
        {
            var latestDate = backupFilesWithDateStamps.Max(b => b.BackupDateTime);
            return backupFilesWithDateStamps.First(b => b.BackupDateTime == latestDate).BackupFileName;
        }
    }
}