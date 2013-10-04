namespace TeamCityBackupTask
{
    public interface HttpBackupRequest 
    {
        void Request(string backupRequestUri);
    }
}