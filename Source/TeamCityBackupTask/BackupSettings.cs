namespace TeamCityBackupTask
{
    public class BackupSettings
    {
        public string BackupRequestUri { get; set; }
        public string BackupStatusUri { get; set; }
        public string BackupRequestUser { get; set; }
        public string BackupRequestPassword { get; set; }
        public string BackupFilesLocation { get; set; }
        public string BackupTargetDestination { get; set; }
    }
}