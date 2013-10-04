namespace TeamCityBackupTask
{
    public class BackupValidationRecord
    {
        private readonly bool _isValid;
        private readonly string _message;

        public bool IsValid { get { return _isValid; } }
        public string Message { get { return _message; } }

        public static BackupValidationRecord Invalid(string message)
        {
            return new BackupValidationRecord(false, message);
        }
        
        public static BackupValidationRecord Valid()
        {
            return new BackupValidationRecord(true);
        }

        private BackupValidationRecord(bool isValid, string message = "")
        {
            _isValid = isValid;
            _message = message;
        }
    }
}