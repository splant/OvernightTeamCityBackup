using System;
using System.Collections.Generic;
using System.IO;
using FakeItEasy;
using NUnit.Framework;
using TeamCityBackupTask;

namespace UnitTests
{
    [TestFixture]
    public class Given_there_are_more_than_the_configured_amount_of_stored_backups_to_keep : CleanUpOldDatabaseBackupsTaskTestBase
    {
        public override void Setup()
        {
            base.Setup();

            var sampleBackupFiles = new []
            {
                "TeamCity_Backup_20131121_165032.zip",
                "TeamCity_Backup_20110323_103232.zip",
                "TeamCity_Backup_20101230_065900.zip",
                "TeamCity_Backup_20111230_065900.zip",
                "TeamCity_Backup_20111121_165032.zip",
                "TeamCity_Backup_20110711_065900.zip",
                "TeamCity_Backup_20110121_065900.zip"
            };

            A.CallTo(() => _fileSystem.GetFileNames(_backupSettings.BackupTargetDestination))
             .Returns(sampleBackupFiles);

            A.CallTo(() => _backupFileDatesQuery.GetDates(sampleBackupFiles))
                .Returns(new[]
                {
                    new BackupFileWithDateStamp("TeamCity_Backup_20131121_165032.zip", new DateTime(2013, 11, 21)),
                    new BackupFileWithDateStamp("TeamCity_Backup_20110323_103232.zip", new DateTime(2011, 03, 23)),
                    new BackupFileWithDateStamp("TeamCity_Backup_20101230_065900.zip", new DateTime(2010, 12, 30)),
                    new BackupFileWithDateStamp("TeamCity_Backup_20111230_065900.zip", new DateTime(2011, 12, 30)),
                    new BackupFileWithDateStamp("TeamCity_Backup_20111124_165032.zip", new DateTime(2011, 11, 24)),
                    new BackupFileWithDateStamp("TeamCity_Backup_20110711_065900.zip", new DateTime(2011, 07, 11)),
                    new BackupFileWithDateStamp("TeamCity_Backup_20110121_065900.zip", new DateTime(2011, 01, 21))
                });

            _backupSettings.NumberOfBackupsToKeep = 4;
        }

        [Test]
        public void Then_we_use_the_file_system_to_remove_the_oldest_files_over_this_amount()
        {
            //When:
            WhenRequestingCleanUp();
            
            //Then:
            A.CallTo(() => _fileSystem.RemoveFile("TeamCity_Backup_20101230_065900.zip")).MustHaveHappened();
            A.CallTo(() => _fileSystem.RemoveFile("TeamCity_Backup_20110121_065900.zip")).MustHaveHappened();
            A.CallTo(() => _fileSystem.RemoveFile("TeamCity_Backup_20110323_103232.zip")).MustHaveHappened();

            A.CallTo(() => _fileSystem.RemoveFile("TeamCity_Backup_20110711_065900.zip")).MustNotHaveHappened();
            A.CallTo(() => _fileSystem.RemoveFile("TeamCity_Backup_20131121_165032.zip")).MustNotHaveHappened();
            A.CallTo(() => _fileSystem.RemoveFile("TeamCity_Backup_20111124_165032.zip")).MustNotHaveHappened();
            A.CallTo(() => _fileSystem.RemoveFile("TeamCity_Backup_20111230_065900.zip")).MustNotHaveHappened();
        }
        
        [Test]
        public void Then_we_send_a_notification_for_each_of_the_files_we_remove()
        {
            //When:
            WhenRequestingCleanUp();
            
            //Then:
            A.CallTo(() => _backupNotifier.SendNotification(
                A<string>.That.Contains("TeamCity_Backup_20101230_065900.zip was removed"))).MustHaveHappened();
            A.CallTo(() => _backupNotifier.SendNotification(
                A<string>.That.Contains("TeamCity_Backup_20110121_065900.zip was removed"))).MustHaveHappened();
            A.CallTo(() => _backupNotifier.SendNotification(
                A<string>.That.Contains("TeamCity_Backup_20110323_103232.zip was removed"))).MustHaveHappened();

            A.CallTo(() => _backupNotifier.SendNotification(A<string>._))
             .MustHaveHappened(Repeated.Exactly.Times(3)); //no other file notifications were sent
        }
    }

    [TestFixture]
    public class Given_there_are_less_backups_than_the_configured_amount_to_keep : CleanUpOldDatabaseBackupsTaskTestBase
    {
        [Test]
        public void Then_we_do_not_remove_any_files()
        {
            //Given:
            A.CallTo(() => _backupFileDatesQuery.GetDates(A<IEnumerable<string>>._))
             .Returns(new[]
                         {
                             new BackupFileWithDateStamp("TeamCity_Backup_20131121_165032.zip", new DateTime(2013, 11, 21)),
                             new BackupFileWithDateStamp("TeamCity_Backup_20110323_103232.zip", new DateTime(2011, 03, 23)),
                             new BackupFileWithDateStamp("TeamCity_Backup_20101230_065900.zip", new DateTime(2010, 12, 30)),
                             new BackupFileWithDateStamp("TeamCity_Backup_20111230_065900.zip", new DateTime(2011, 12, 30))
                         });

            _backupSettings.NumberOfBackupsToKeep = 4;

            BackupCleanUp backupCleanUp = GetSUT();

            //When:
            backupCleanUp.DoCleanUp();

            //Then:
            A.CallTo(() => _fileSystem.RemoveFile(A<string>._)).MustNotHaveHappened();
        }
    }

    [TestFixture]
    public class And_we_encounter_a_problem_trying_to_remove_a_backup_file : Given_there_are_more_than_the_configured_amount_of_stored_backups_to_keep
    {
        [Test]
        public void Then_we_send_a_notification_with_the_problem_details()
        {
            //Given:
            string expectedNotificationDetails = "Cannot access the disk";
            string expectedBackupFileName = "TeamCity_Backup_20101230_065900.zip";
            string expectedNotificationMessage = string.Format(
                "Could not remove backup file {0} from target destination", expectedBackupFileName);

            A.CallTo(() => _fileSystem.RemoveFile(A<string>._))
             .Throws(new IOException(expectedNotificationDetails));

            //When:
            WhenRequestingCleanUp();

            //Then:
            VerifyNotificationSentContaining(expectedNotificationMessage);
            VerifyNotificationSentContaining(expectedNotificationDetails);
        }

        private void VerifyNotificationSentContaining(string content)
        {
            A.CallTo(() => _backupNotifier.SendNotification(A<string>.That.Contains(content)))
                .MustHaveHappened();
        }
    }
    
    public abstract class CleanUpOldDatabaseBackupsTaskTestBase
    {
        protected FileSystem _fileSystem;
        protected BackupSettings _backupSettings;
        protected BackupCleanUp _backupCleanUp;
        protected BackupFileDatesQuery _backupFileDatesQuery;
        protected BackupNotifier _backupNotifier;

        [SetUp]
        public virtual void Setup()
        {
            _fileSystem = A.Fake<FileSystem>();
            _backupFileDatesQuery = A.Fake<BackupFileDatesQuery>();
            _backupNotifier = A.Fake<BackupNotifier>();

            _backupSettings = new BackupSettings
            {
                BackupTargetDestination = "A\\Target\\Destination"
            };

            _backupCleanUp = GetSUT();
        }

        public BackupCleanUp GetSUT()
        {
            return new CleanUpOldDatabaseBackupsTask(_fileSystem, _backupFileDatesQuery, _backupSettings, _backupNotifier);
        }

        protected void WhenRequestingCleanUp()
        {
            _backupCleanUp.DoCleanUp();
        }
    }
}
