using System;

namespace TeamCityBackupTask
{
    public class HandledBackupRequest : BackupRequest 
    {
        private readonly BackupSettings _backupSettings;
        private readonly HttpBackupRequest _httpBackupRequest;

        public HandledBackupRequest(BackupSettings backupSettings, HttpBackupRequest httpBackupRequest)
        {
            _backupSettings = backupSettings;
            _httpBackupRequest = httpBackupRequest;
        }

        public void RequestBackup()
        {
            try
            {
                _httpBackupRequest.Request(_backupSettings);
            }
            catch (Exception exception)
            {
                throw new BackupFailed(ExtractFullExceptionMessage(exception));
            }
        }

        private string ExtractFullExceptionMessage(Exception exception)
        {
            string exceptionMessage = exception.Message;
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
                exceptionMessage += " : " + exception.Message;
            }
            return exceptionMessage;
        }
    }
}