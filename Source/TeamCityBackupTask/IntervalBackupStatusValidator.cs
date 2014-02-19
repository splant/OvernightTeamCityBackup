namespace TeamCityBackupTask
{
    public class IntervalBackupStatusValidator : BackupValidator
    {
        private readonly string _requestedStatus;
        private readonly int _secondsToWait;
        private readonly int _secondsInterval;
        private readonly HttpBackupStatus _httpBackupStatus;
        private readonly IntervalHandler _intervalHandler;

        public IntervalBackupStatusValidator(
            string requestedStatus, int secondsToWait, int secondsInterval, 
            HttpBackupStatus httpBackupStatus, IntervalHandler intervalHandler)
        {
            _requestedStatus = requestedStatus;
            _secondsToWait = secondsToWait;
            _secondsInterval = secondsInterval;
            _httpBackupStatus = httpBackupStatus;
            _intervalHandler = intervalHandler;
        }

        public BackupValidationRecord GetBackupValidation()
        {
            if (_httpBackupStatus.GetBackupStatus() == _requestedStatus)
                return BackupValidationRecord.Valid();

            if (_secondsToWait <= 0)
                return BackupValidationRecord.Invalid(string.Format(
                    "No backup with requested state: {0} was found in the elapsed allowed backup period", 
                    _requestedStatus));

            _intervalHandler.WaitInterval(_secondsInterval);

            int reducedTimeToWait = _secondsToWait - _secondsInterval;

            return new IntervalBackupStatusValidator(
                _requestedStatus, reducedTimeToWait, _secondsInterval, _httpBackupStatus, _intervalHandler)
                    .GetBackupValidation();
        }
    }
}