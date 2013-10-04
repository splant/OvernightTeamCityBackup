namespace TeamCityBackupTask
{
    public interface BackupNotifier 
    {
        void SendNotification(string message);
    }
}