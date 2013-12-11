using System;
using System.Linq;

namespace TeamCityBackupTask
{
    public class BackupExistsValidator : BackupValidator 
    {
        private readonly BackupSettings _backupSettings;
        private readonly FileSystem _fileSystem;
        private readonly BackupFileDatesQuery _backupFileDatesQuery;
        private readonly CurrentDateProvider _currentDateProvider;

        public BackupExistsValidator(
            BackupSettings backupSettings, 
            FileSystem fileSystem, 
            BackupFileDatesQuery backupFileDatesQuery, 
            CurrentDateProvider currentDateProvider)
        {
            _backupSettings = backupSettings;
            _fileSystem = fileSystem;
            _backupFileDatesQuery = backupFileDatesQuery;
            _currentDateProvider = currentDateProvider;
        }

        public BackupValidationRecord GetBackupValidation()
        {
            var backupFileNames = _fileSystem.GetFileNames(_backupSettings.BackupFilesLocation);
            var dateTimes = _backupFileDatesQuery.GetDates(backupFileNames);

            if (dateTimes.Any(OccurredWithinTheLastTwoHours())) 
                return BackupValidationRecord.Valid();

            return BackupValidationRecord.Invalid("No backup file created within the last two hours was found");
        }

        private Func<BackupFileWithDateStamp, bool> OccurredWithinTheLastTwoHours()
        {
            return backupFileWithDateStamp => 
                (_currentDateProvider.Now - backupFileWithDateStamp.BackupDateTime).TotalHours < 2;
        }
    }
}