using System;
using System.Threading;

namespace TeamCityBackupTask
{
    public class ThreadSleepInterval : IntervalHandler 
    {
        public void WaitInterval(int seconds)
        {
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
        }
    }
}