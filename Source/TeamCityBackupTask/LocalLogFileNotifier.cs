using System;
using System.IO;

namespace TeamCityBackupTask
{
    public class LocalLogFileNotifier : BackupNotifier
    {
        public void SendNotification(string message)
        {
            using (var streamWriter = new StreamWriter("c:\\teamcity_backup_log.txt", true))
            {
                streamWriter.WriteLine(DateTime.UtcNow + ":    " + message);
            }
        }
    }
}
