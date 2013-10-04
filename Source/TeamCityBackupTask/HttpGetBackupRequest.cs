namespace TeamCityBackupTask
{
    public interface HttpGetBackupRequest 
    {
        void Get(string backupRequestUri);
    }
}