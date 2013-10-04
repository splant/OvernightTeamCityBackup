using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace TeamCityBackupTask
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpRestBackupRequest httpRestBackupRequest = new HttpRestBackupRequest
            (
                new BackupSettings { BackupRequestUri = ConfigurationManager.AppSettings["BackupRequestUri"] }, 
                new RestClientBackupRequest()
            );

            httpRestBackupRequest.RequestBackup();

            Console.ReadKey();
        }
    }
}
