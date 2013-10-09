namespace TeamCityBackupTask
{
//    public class BackupFinishedValidator : BackupValidator
//    {
//        private const string IDLE_STATUS = "Idle";
//        private readonly HttpBackupRunningStatus _httpBackupRunningStatus;
//        private readonly IntervalHandler _intervalHandler;
//        private readonly int _secondsToWait;
//        private readonly int _secondsInterval;
//
//        public BackupFinishedValidator(
//            HttpBackupRunningStatus httpBackupRunningStatus,
//            IntervalHandler intervalHandler,
//            int secondsToWait, int secondsInterval)
//        {
//            _httpBackupRunningStatus = httpBackupRunningStatus;
//            _intervalHandler = intervalHandler;
//            _secondsToWait = secondsToWait;
//            _secondsInterval = secondsInterval;
//        }
//
//        public BackupValidationRecord GetBackupValidation() //Is this immutable
//        {
//            if (_httpBackupRunningStatus.GetBackupStatus() == IDLE_STATUS)
//                return BackupValidationRecord.Valid();
//
//            if (_secondsToWait <= 0)
//                return BackupValidationRecord.Invalid("The backup did not finish in the allowed time period"); //maybe include time period
//
//            _intervalHandler.WaitInterval(_secondsInterval);
//
//            return new BackupFinishedValidator(_httpBackupRunningStatus, _intervalHandler, _secondsToWait - _secondsInterval, _secondsInterval)
//                .GetBackupValidation();
//        }
//    }
//
//
//    public interface HttpBackupRunningStatus
//    {
//        string GetBackupStatus();
//    }
}

//need to also assert that the backup is running before checking that it has finshed (2 parts of composite)
//other part validates the file exists (check for file created on disk within last 5 minutes) 
