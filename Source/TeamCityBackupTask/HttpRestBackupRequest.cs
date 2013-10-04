using System;

namespace TeamCityBackupTask
{
    public class HttpRestBackupRequest : BackupRequest 
    {
        private readonly BackupSettings _backupSettings;
        private readonly HttpGetBackupRequest _httpGetBackupRequest;

        public HttpRestBackupRequest(BackupSettings backupSettings, HttpGetBackupRequest httpGetBackupRequest)
        {
            _backupSettings = backupSettings;
            _httpGetBackupRequest = httpGetBackupRequest;
        }

        public void RequestBackup()
        {
            try
            {
                _httpGetBackupRequest.Get(_backupSettings.BackupRequestUri);
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