namespace TeamCityBackupTask
{
    public class BackupController 
    {
        private readonly BackupProcess _backupProcess;
        private readonly BackupStorage _backupStorage;
        private readonly BackupNotifier _backupNotifier;

        public BackupController(
            BackupProcess backupProcess, BackupStorage backupStorage, BackupNotifier backupNotifier)
        {
            _backupProcess = backupProcess;
            _backupStorage = backupStorage;
            _backupNotifier = backupNotifier;
        }

        public void Backup()
        {
            try
            {
                DoBackup();
            }
            catch (BackupFailed backupFailedException)
            {
                _backupNotifier.SendNotification("Backup failed: " + backupFailedException.Message);
            }
            catch (StorageFailed storageFailedException)
            {
                _backupNotifier.SendNotification("Backup storage failed: " + storageFailedException.Message);
            }
        }

        private void DoBackup()
        {
            _backupProcess.ExecuteBackup();
            _backupStorage.StoreBackup();
            _backupNotifier.SendNotification("Backup was a success");
        }
    }
}