using System;
using System.IO;
using FakeItEasy;
using NUnit.Framework;
using TeamCityBackupTask;

namespace UnitTests
{
    [TestFixture]
    public class Given_a_recent_backup_exists : CopyLatestBackupStorageTaskTestBase
    {
        public string _recentBackupFileName = "TeamCity_Backup_20131121_165032.zip";

        public override void Setup()
        {
            base.Setup();
            A.CallTo(() => _latestRecentBackupQuery.GetName()).Returns(_recentBackupFileName);
        }
        
        [Test]
        public void We_use_a_LatestRecentBackupQuery_to_get_the_full_file_name()
        {
            //Given:
            CopyLatestBackupStorageTask copyLatestBackupStorageTask = GetSUT();

            //When:
            copyLatestBackupStorageTask.StoreBackup();

            //Then:
            A.CallTo(() => _latestRecentBackupQuery.GetName()).MustHaveHappened();
        }
        
        [Test]
        public void We_ask_the_file_system_to_copy_the_backup_file_to_a_location_specified_in_the_backup_settings()
        {
            //Given:
            CopyLatestBackupStorageTask copyLatestBackupStorageTask = GetSUT();

            //When:
            copyLatestBackupStorageTask.StoreBackup();

            //Then:
            A.CallTo(() => _fileSystem.CopyFile(
                _recentBackupFileName, A<string>.That.Contains(_defaultSettings.BackupTargetDestination)))
             .MustHaveHappened();
        }

        [TestCase("c:\\somewhere\\else\\TeamCity_Backup_20131121_165032.zip", "c:\\somewhere\\out\\there", 
            "c:\\somewhere\\out\\there\\TeamCity_Backup_20131121_165032.zip")]
        [TestCase("F:\\a\\path\\TeamCity_Backup_20091230_065900.zip", "E:\\another\\path\\somewhere",
            "E:\\another\\path\\somewhere\\TeamCity_Backup_20091230_065900.zip")]
        [TestCase("F:\\a\\path\\backup_file_20091230_065900.zip", "E:\\another\\path\\somewhere",
            "E:\\another\\path\\somewhere\\backup_file_20091230_065900.zip")]
        public void Then_the_target_destination_is_a_combination_of_the_destination_and_the_expected_file_name(
            string latestBackupFilePath, string baseDestinationPath, string expectedDestinationFilePath)
        {
            //Given:
            A.CallTo(() => _latestRecentBackupQuery.GetName()).Returns(latestBackupFilePath);
            BackupSettings backupSettings = new BackupSettings { BackupTargetDestination = baseDestinationPath };

            CopyLatestBackupStorageTask copyLatestBackupStorageTask = GetSUT(backupSettings);

            //When:
            copyLatestBackupStorageTask.StoreBackup();

            //Then:
            A.CallTo(() => _fileSystem.CopyFile(latestBackupFilePath, expectedDestinationFilePath))
             .MustHaveHappened();
        }

        [TestFixture]
        public class Given_a_problem_is_encountered_while_copying_the_backup_file : Given_a_recent_backup_exists
        {
            public object[] ExceptionTestCases = new object[]
            {
                new object[] { new Exception("ExceptionMessage") },
                new object[] { new IOException("IOExceptionMessage") },
                new object[] { new AccessViolationException("AccessExceptionMessage") }
            };

            [TestCaseSource("ExceptionTestCases")]
            public void Then_a_StorageFailed_exception_is_thrown_containing_the_error_message(Exception copyingException)
            {
                //Given:
                CopyLatestBackupStorageTask copyLatestBackupStorageTask = GetSUT();

                A.CallTo(() => _fileSystem.CopyFile(A<string>._, A<string>._)).Throws(copyingException);

                //When:
                TestDelegate storingBackup = () => copyLatestBackupStorageTask.StoreBackup();

                //Then:
                Assert.That(storingBackup,
                    Throws.InstanceOf<StorageFailed>()
                          .With.Message.StringContaining(copyingException.AllMessagesWithStackTraces()));
            }
        }
    }

    [TestFixture]
    public class Given_no_recent_backup_file_exists : CopyLatestBackupStorageTaskTestBase
    {
        private CopyLatestBackupStorageTask _copyLatestBackupStorageTask;

        public override void Setup()
        {
            base.Setup();
            A.CallTo(() => _latestRecentBackupQuery.GetName()).Returns("");

            _copyLatestBackupStorageTask = GetSUT();
        }
        
        [Test]
        public void Then_a_StorageFailed_exception_is_thrown()
        {
            //When:
            TestDelegate storingBackup = () => _copyLatestBackupStorageTask.StoreBackup();

            //Then:
            Assert.That(storingBackup, 
                Throws.InstanceOf<StorageFailed>().With.Message.StringContaining("No valid backup file"));
        }
    }

    public abstract class CopyLatestBackupStorageTaskTestBase
    {
        protected LatestRecentBackupQuery _latestRecentBackupQuery;
        protected FileSystem _fileSystem;
        protected BackupSettings _defaultSettings;

        [SetUp]
        public virtual void Setup()
        {
            _latestRecentBackupQuery = A.Fake<LatestRecentBackupQuery>();
            _fileSystem = A.Fake<FileSystem>();

            _defaultSettings = new BackupSettings { BackupTargetDestination = "TargetDestination" };
        }
        
        public CopyLatestBackupStorageTask GetSUT(BackupSettings backupSettings = null)
        {
            return new CopyLatestBackupStorageTask(
                _latestRecentBackupQuery, _fileSystem, backupSettings ?? _defaultSettings);
        }
    }
}
