namespace TeamCityBackupTask
{
    public class BackupFinishedValidator : IntervalBackupStatusValidator
    {
        public BackupFinishedValidator(int secondsToWait, int secondsInterval, 
                                       HttpBackupStatus httpBackupStatus, IntervalHandler intervalHandler) 
            : base("Idle", secondsToWait, secondsInterval, httpBackupStatus, intervalHandler) {}
    }
}