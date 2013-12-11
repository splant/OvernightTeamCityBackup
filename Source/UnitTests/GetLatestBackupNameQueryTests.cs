using System;
using System.Collections.Generic;
using FakeItEasy;
using NUnit.Framework;
using TeamCityBackupTask;

namespace UnitTests
{
    [TestFixture]
    public class Given_we_have_recent_backup_files_available : GetLatestBackupNameQueryTestBase
    {      
        [Test]
        public void Then_we_use_the_file_system_to_get_their_names_from_the_backup_files_location()
        {
            //When:
            RequestingLatestBackupName();

            //Then:
            A.CallTo(() => _fileSystem.GetFileNames(_backupSettings.BackupFilesLocation)).MustHaveHappened();
        }

        [Test]
        public void Then_we_use_the_BackupFileDatesQuery_to_get_the_datetime_stamp_of_when_these_backups_occured()
        {
            //Given:
            var availableBackupFileNames = new[]
                {
                    "TeamCity_Backup_20131121_165032.zip",
                    "TeamCity_Backup_20110323_103232.zip",
                    "TeamCity_Backup_20091230_065900.zip"
                };

            A.CallTo(() => _fileSystem.GetFileNames(A<string>._)).Returns(availableBackupFileNames);
            
            //When:
            RequestingLatestBackupName();

            //Then:
            A.CallTo(() => _backupFileDatesQuery.GetDates(availableBackupFileNames)).MustHaveHappened();
        }

        public static object[] LatestBackupTestCases = new object[]
            {
                new object []
                { 
                    new[]
                    {                   
                        new BackupFileWithDateStamp("TeamCity_Backup_20131121_165032.zip", 
                            new DateTime(2013, 11, 21, 16, 50, 32)),
                        new BackupFileWithDateStamp("TeamCity_Backup_20110323_103232.zip", 
                            new DateTime(2011, 03, 23, 10, 32, 32)),
                        new BackupFileWithDateStamp("TeamCity_Backup_20091230_065900.zip", 
                            new DateTime(2009, 12, 30, 06, 59, 00))
                    },
                    "TeamCity_Backup_20131121_165032.zip"
                },
                new object []
                { 
                    new[]
                    {                   
                        new BackupFileWithDateStamp("TeamCity_Backup_20121121_165032.zip", 
                            new DateTime(2012, 11, 21, 16, 50, 32)),
                        new BackupFileWithDateStamp("TeamCity_Backup_20120323_103232.zip", 
                            new DateTime(2012, 03, 23, 10, 32, 32)),
                        new BackupFileWithDateStamp("TeamCity_Backup_20121230_065900.zip", 
                            new DateTime(2012, 12, 30, 06, 59, 00))
                    },
                    "TeamCity_Backup_20121230_065900.zip"
                }
            };

        [TestCaseSource("LatestBackupTestCases")]
        public void Then_we_return_the_filename_for_the_highest_date_out_of_those_returned_by_the_BackupFileDatesQuery(
            IEnumerable<BackupFileWithDateStamp> backupFilesWithDates, string expectedBackupFileName)
        {
            //Given:

            A.CallTo(() => _backupFileDatesQuery.GetDates(A<IEnumerable<string>>._)).Returns(backupFilesWithDates);

            //When:
            var latestBackupName = RequestingLatestBackupName();

            //Then:
            Assert.That(latestBackupName, Is.EqualTo(expectedBackupFileName));
        }

        private string RequestingLatestBackupName()
        {
            //Given:
            GetLatestBackupNameQuery getLatestBackupName = GetSUT();

            //When:
            return getLatestBackupName.GetName();
        }
    }

    [TestFixture]
    public class Given_there_are_no_valid_backups_available : GetLatestBackupNameQueryTestBase
    {
        private GetLatestBackupNameQuery _getLatestBackupNameQuery;

        public override void Setup()
        {
            base.Setup();

            A.CallTo(() => _fileSystem.GetFileNames(A<string>._)).Returns(new string[]{});

            A.CallTo(() => _backupFileDatesQuery.GetDates(A<IEnumerable<string>>._))
             .Returns(new BackupFileWithDateStamp[]{});

            _getLatestBackupNameQuery = GetSUT();
        } 
        
        [Test]
        public void Then_a_blank_string_is_returned()
        {
            //When:
            var latestBackupFileName = _getLatestBackupNameQuery.GetName();

            //Then:
            Assert.That(latestBackupFileName, Is.EqualTo(""));
        }
    }
    
    public abstract class GetLatestBackupNameQueryTestBase
    {
        protected FileSystem _fileSystem;
        protected BackupSettings _backupSettings;
        protected BackupFileDatesQuery _backupFileDatesQuery;

        [SetUp]
        public virtual void Setup()
        {
            _fileSystem = A.Fake<FileSystem>();
            _backupFileDatesQuery = A.Fake<BackupFileDatesQuery>();

            _backupSettings = new BackupSettings
                {
                    BackupFilesLocation = "a\\files\\location"
                };
        }
        
        public GetLatestBackupNameQuery GetSUT()
        {
            return new GetLatestBackupNameQuery(_backupSettings, _fileSystem, _backupFileDatesQuery);
        }
    }
}
