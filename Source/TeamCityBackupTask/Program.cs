using System;
using System.Configuration;

namespace TeamCityBackupTask
{
    class Program
    {
        private const int ONE_MINUTE_DURATION = 60;
        private const int FIVE_SECOND_INTERVAL = 5;
        private const int THIRTY_SECOND_INTERVAL = 30;

        private static int _timeBackupAllowedToTake;

        static void Main(string[] args)
        {
            _timeBackupAllowedToTake = GetApplicationSetting<int>("TotalAllowedBackupTimeInSeconds");
            
            BackupController backupController = BuildBackupController();
            backupController.Backup();
        }

        public static BackupController BuildBackupController()
        {
            BackupSettings applicationBackupSettings = new BackupSettings
                {
                    BackupRequestUri = GetApplicationSetting<string>("BackupRequestUri"),
                    BackupStatusUri = GetApplicationSetting<string>("BackupStatusUri"),
                    BackupFilesLocation = GetApplicationSetting<string>("BackupFilesLocation"),
                    BackupTargetDestination = GetApplicationSetting<string>("BackupTargetDestination"),
                    BackupRequestUser = GetApplicationSetting<string>("BackupRequestUser"),
                    BackupRequestPassword = GetApplicationSetting<string>("BackupRequestPassword"),
                    NumberOfBackupsToKeep = GetApplicationSetting<int>("NumberOfBackupsToKeep"),
                };

            BackupNotifier backupNotifier = new LocalLogFileNotifier();

            HttpBackupRequest httpBackupRequest = new RestfulPostBackupRequest();
            BackupRequest backupRequest = new HandledBackupRequest(applicationBackupSettings, httpBackupRequest);

            IntervalHandler intervalHandler = new ThreadSleepInterval();
            HttpBackupStatus httpBackupStatus = new RestfulGetBackupStatus(applicationBackupSettings);
            FileSystem fileSystem = new WindowsFileSystem();
            BackupFileDatesQuery backupFileDatesQuery = new TeamCityBackupFileDatesQuery();
            CurrentDateProvider currentDateProvider = new FrameworkCurrentDateProvider();

            BackupValidator compositeBackupValidators = new CompositeBackupValidators(new BackupValidator[]
                {
                    new BackupStartedValidator(
                        ONE_MINUTE_DURATION, FIVE_SECOND_INTERVAL, httpBackupStatus, intervalHandler), 
                    new BackupFinishedValidator(
                        _timeBackupAllowedToTake, THIRTY_SECOND_INTERVAL, httpBackupStatus, intervalHandler),
                    new BackupExistsValidator(
                        applicationBackupSettings, fileSystem, backupFileDatesQuery, currentDateProvider) 
                });

            BackupProcess backupProcess = new ValidatedBackupProcess(backupRequest, compositeBackupValidators);

            LatestRecentBackupQuery latestRecentBackupQuery = new GetLatestBackupNameQuery(
                applicationBackupSettings, fileSystem, backupFileDatesQuery);

            BackupStorage backupStorage = new CopyLatestBackupStorageTask(
                latestRecentBackupQuery, fileSystem, applicationBackupSettings);

            BackupCleanUp backupCleanUp = new CleanUpOldDatabaseBackupsTask(
                fileSystem, backupFileDatesQuery, applicationBackupSettings, backupNotifier);

            return new BackupController(backupProcess, backupStorage, backupCleanUp, backupNotifier);
        }

        public static T GetApplicationSetting<T>(string settingName)
        {
            object value = ConfigurationManager.AppSettings[settingName]; 
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
