using System;
using System.Net.Http;

namespace TeamCityBackupTask
{
    public class RestClientBackupRequest : HttpBackupRequest 
    {
        public void Request(string backupRequestUri)
        {
            var message = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(backupRequestUri)
                };

            var client = new HttpClient();
            client.SendAsync(message).ContinueWith(task =>
                {
                    task.Result.EnsureSuccessStatusCode();

                    var jsonHypermediaResult = task.Result.Content.ReadAsStringAsync();
                });	
        }
    }
}