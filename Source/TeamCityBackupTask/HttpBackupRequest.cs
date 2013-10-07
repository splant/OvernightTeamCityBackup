namespace TeamCityBackupTask
{
    public interface HttpBackupRequest 
    {
        void Request(BackupSettings backupSettings);
    }
}