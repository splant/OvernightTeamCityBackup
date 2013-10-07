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
            HandledBackupRequest handledBackupRequest = new HandledBackupRequest
            (
                new BackupSettings
                    {
                        BackupRequestUri = GetAppSetting("BackupRequestUri"),

                    }, 
                new RestfulPostBackupRequest()
            );

            handledBackupRequest.RequestBackup();

            Console.ReadKey();
        }

        private static string GetAppSetting(string appSettingName)
        {
            return ConfigurationManager.AppSettings[appSettingName];
        }
    }
}
