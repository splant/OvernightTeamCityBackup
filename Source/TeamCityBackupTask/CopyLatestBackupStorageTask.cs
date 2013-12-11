using System;
using System.IO;

namespace TeamCityBackupTask
{
    public class CopyLatestBackupStorageTask : BackupStorage 
    {
        private readonly LatestRecentBackupQuery _latestRecentBackupQuery;
        private readonly FileSystem _fileSystem;
        private readonly BackupSettings _backupSettings;

        public CopyLatestBackupStorageTask(
            LatestRecentBackupQuery latestRecentBackupQuery, 
            FileSystem fileSystem, 
            BackupSettings backupSettings)
        {
            _latestRecentBackupQuery = latestRecentBackupQuery;
            _fileSystem = fileSystem;
            _backupSettings = backupSettings;
        }

        public void StoreBackup()
        {
            var lastBackupFile = TryGetLastBackupFile();
            var fileNameSegment = lastBackupFile.Substring(lastBackupFile.LastIndexOf('\\') + 1);

            TryCopyBackupFile(lastBackupFile, fileNameSegment); 
        }

        private string TryGetLastBackupFile()
        {
            var lastBackupFile = _latestRecentBackupQuery.GetName();

            if (lastBackupFile == "")
                throw new StorageFailed("No valid backup file was available to copy to the target destination");

            return lastBackupFile;
        }

        private void TryCopyBackupFile(string lastBackupFile, string fileNameSegment)
        {
            try
            {
                _fileSystem.CopyFile(lastBackupFile,
                                     Path.Combine(_backupSettings.BackupTargetDestination, fileNameSegment));
            }
            catch (Exception exception)
            {
                throw new StorageFailed("Error copying backup file: " + exception.AllMessagesWithStackTraces());
            }
        }
    }
}