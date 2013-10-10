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
             //how to know if the status is definitely invalid?
            IntervalBackupStatusValidator intervalBackupStatusValidator = 
                new IntervalBackupStatusValidator("not found", 60, 5, 
                    new RestfulGetBackupStatus(
                        new BackupSettings
                            {

                            }), 
                    new ThreadSleepInterval());

            intervalBackupStatusValidator.GetBackupValidation();

//            HandledBackupRequest handledBackupRequest = new HandledBackupRequest
//            (
//                new BackupSettings
//                    {
//                        BackupRequestUri = GetAppSetting("BackupRequestUri"),
//
//                    }, 
//                new RestfulPostBackupRequest()
//            );
//
//            handledBackupRequest.RequestBackup();

            Console.ReadKey();
        }

        private static string GetAppSetting(string appSettingName)
        {
            return ConfigurationManager.AppSettings[appSettingName];
        }
    }

    public class StubHttpbackupStatus : HttpBackupStatus
    {
        public string GetBackupStatus()
        {
            return "Something else";
        }
    }
}
