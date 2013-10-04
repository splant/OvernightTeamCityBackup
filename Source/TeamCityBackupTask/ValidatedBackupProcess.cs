namespace TeamCityBackupTask
{
    public class ValidatedBackupProcess : BackupProcess
    {
        private readonly BackupRequest _backupRequest;
        private readonly BackupValidator _backupValidator;

        public ValidatedBackupProcess(BackupRequest backupRequest, BackupValidator backupValidator)
        {
            _backupRequest = backupRequest;
            _backupValidator = backupValidator;
        }

        public void ExecuteBackup()
        {
            _backupRequest.RequestBackup();

            var backupValidation = _backupValidator.GetBackupValidation();

            if (BackupIsInvalid(backupValidation))
                throw new BackupFailed(backupValidation.Message);
        }

        private bool BackupIsInvalid(BackupValidationRecord backupValidationRecord)
        {
            return !backupValidationRecord.IsValid;
        }
    }
}