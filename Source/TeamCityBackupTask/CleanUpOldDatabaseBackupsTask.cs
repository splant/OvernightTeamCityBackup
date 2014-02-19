using System;
using System.Linq;

namespace TeamCityBackupTask
{
    public class CleanUpOldDatabaseBackupsTask : BackupCleanUp 
    {
        private readonly FileSystem _fileSystem;
        private readonly BackupFileDatesQuery _backupFileDatesQuery;
        private readonly BackupSettings _backupSettings;
        private readonly BackupNotifier _backupNotifier;

        public CleanUpOldDatabaseBackupsTask(
            FileSystem fileSystem, 
            BackupFileDatesQuery backupFileDatesQuery, 
            BackupSettings backupSettings, 
            BackupNotifier backupNotifier)
        {
            _fileSystem = fileSystem;
            _backupFileDatesQuery = backupFileDatesQuery;
            _backupSettings = backupSettings;
            _backupNotifier = backupNotifier;
        }

        public void DoCleanUp()
        {
            var backupFilesAtTargetDestination = _fileSystem.GetFileNames(_backupSettings.BackupTargetDestination);

            var backupsThatCanBeRemoved = _backupFileDatesQuery
                .GetDates(backupFilesAtTargetDestination)
                .OrderByDescending(backupFiles => backupFiles.BackupDateTime).ToList()
                .Skip(_backupSettings.NumberOfBackupsToKeep);

            foreach (var backupFile in backupsThatCanBeRemoved)
                TryRemoveBackupFile(backupFile);
        }

        //handles failures itself quietly as the impact here is less than if the backup process or copy fail
        private void TryRemoveBackupFile(BackupFileWithDateStamp backupFile)
        {
            try
            {
                _fileSystem.RemoveFile(backupFile.BackupFileName);
                _backupNotifier.SendNotification(
                    string.Format("An older backup file {0} was removed", backupFile.BackupFileName));
            }
            catch (Exception exception)
            {
                string notificationTitle = string.Format(
                    "Could not remove backup file {0} from target destination", backupFile.BackupFileName);

                string notificationMessage = notificationTitle + 
                                             Environment.NewLine + 
                                             Environment.NewLine +
                                             exception.AllMessagesWithStackTraces();

                _backupNotifier.SendNotification(notificationMessage);
            }
        }
    }
}