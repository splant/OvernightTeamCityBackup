namespace TeamCityBackupTask
{
    public class BackupStartedValidator : IntervalBackupStatusValidator
    {
        public BackupStartedValidator(int secondsToWait, int secondsInterval, 
                                      HttpBackupStatus httpBackupStatus, IntervalHandler intervalHandler) 
            : base("Running", secondsToWait, secondsInterval, httpBackupStatus, intervalHandler) {}
    }
}