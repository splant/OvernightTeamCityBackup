using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace TeamCityBackupTask
{
    public class RestfulGetBackupStatus : HttpBackupStatus
    {
        private readonly BackupSettings _backupSettings;

        public RestfulGetBackupStatus(BackupSettings backupSettings)
        {
            _backupSettings = backupSettings;
        }

        public string GetBackupStatus()
        {
            var client = new HttpClient();
            var overallRequestTask = client
                        .SendAsync(GetBackupStatusRequestMessage())
                        .ContinueWith(requestMessageTask =>
                        {
                            requestMessageTask.Result.EnsureSuccessStatusCode();
                            var response = requestMessageTask.Result;

                            var statusTask = response.Content.ReadAsStringAsync();
                            statusTask.Wait();

                            return statusTask.Result;
                        });

            overallRequestTask.Wait();

            return overallRequestTask.Result;
        }

        private HttpRequestMessage GetBackupStatusRequestMessage()
        {
            var message = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(_backupSettings.BackupStatusUri),
                };

            AddBasicAuthorizationHeader(message);

            return message;
        }

        private void AddBasicAuthorizationHeader(HttpRequestMessage message)
        {
            string plainTextUserCredentials = string.Format(
                "{0}:{1}", _backupSettings.BackupRequestUser, _backupSettings.BackupRequestPassword);

            string encodedUserCredentials =
                Convert.ToBase64String(Encoding.ASCII.GetBytes(plainTextUserCredentials));

            message.Headers.Authorization = new AuthenticationHeaderValue("Basic", encodedUserCredentials);
        }
    }
}
