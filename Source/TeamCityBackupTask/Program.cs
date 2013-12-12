using System;
using System.Configuration;

namespace TeamCityBackupTask
{
    class Program
    {
        private const int ONE_MINUTE_DURATION = 60;
        private const int THIRTY_MINUTE_DURATION = 1800;
        private const int FIVE_SECOND_INTERVAL = 5;
        private const int THIRTY_SECOND_INTERVAL = 30;

        static void Main(string[] args)
        {
            BackupController backupController = BuildBackupController();
            backupController.Backup();

            Console.ReadKey();
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
                };

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
                        THIRTY_MINUTE_DURATION, THIRTY_SECOND_INTERVAL, httpBackupStatus, intervalHandler),
                    new BackupExistsValidator(
                        applicationBackupSettings, fileSystem, backupFileDatesQuery, currentDateProvider) 
                });

            BackupProcess backupProcess = new ValidatedBackupProcess(backupRequest, compositeBackupValidators);

            LatestRecentBackupQuery latestRecentBackupQuery = new GetLatestBackupNameQuery(
                applicationBackupSettings, fileSystem, backupFileDatesQuery);

            BackupStorage backupStorage = new CopyLatestBackupStorageTask(
                latestRecentBackupQuery, fileSystem, applicationBackupSettings);

            BackupNotifier backupNotifier = new LocalLogFileNotifier();

            return new BackupController(backupProcess, backupStorage, backupNotifier);
        }

        public static T GetApplicationSetting<T>(string settingName)
        {
            object value = ConfigurationManager.AppSettings[settingName]; 
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
