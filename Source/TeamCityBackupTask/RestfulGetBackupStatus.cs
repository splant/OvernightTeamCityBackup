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
            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_backupSettings.BackupStatusUri),
            };

            AddBasicAuthorizationHeader(message);

            var client = new HttpClient();
            var task = client.SendAsync(message).ContinueWith(taskWithMsg =>
                {
                    taskWithMsg.Result.EnsureSuccessStatusCode();
                    var response = taskWithMsg.Result;

                    var stringTask = response.Content.ReadAsStringAsync();
                    stringTask.Wait();
                    return stringTask.Result;
                });

            task.Wait();
            return task.Result;
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
