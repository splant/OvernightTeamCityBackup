using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace TeamCityBackupTask
{
    public class RestfulPostBackupRequest : HttpBackupRequest 
    {
        public void Request(BackupSettings backupSettings)
        {
            var message = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(backupSettings.BackupRequestUri),
                };

            AddBasicAuthorizationHeader(backupSettings, message);

            var client = new HttpClient();
            client.SendAsync(message).ContinueWith(task => { task.Result.EnsureSuccessStatusCode(); });	
        }

        private void AddBasicAuthorizationHeader(BackupSettings backupSettings, HttpRequestMessage message)
        {
            string plainTextUserCredentials = string.Format(
                "{0}:{1}", backupSettings.BackupRequestUser, backupSettings.BackupRequestPassword);

            string encodedUserCredentials =
                Convert.ToBase64String(Encoding.ASCII.GetBytes(plainTextUserCredentials));

            message.Headers.Authorization = new AuthenticationHeaderValue("Basic", encodedUserCredentials);
        }
    }
}