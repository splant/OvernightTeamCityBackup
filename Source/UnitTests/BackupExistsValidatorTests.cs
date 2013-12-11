using System;
using System.Collections.Generic;
using FakeItEasy;
using NUnit.Framework;
using TeamCityBackupTask;

namespace UnitTests
{
    [TestFixture]
    public class Given_we_are_trying_to_verify_a_backup_exists_on_the_backup_path : BackupExistsValidatorTestBase
    {
        [Test]
        public void Then_a_list_of_all_backup_files_should_be_requested()
        {
            //Given:
            string backupFilesLocation = "someLocation";
            BackupSettings backupSettings = new BackupSettings
                {
                    BackupFilesLocation = backupFilesLocation
                };

            //When:
            whenABackupIsExpected(backupSettings);

            //Then:
            A.CallTo(() => _fileSystem.GetFileNames(backupFilesLocation)).MustHaveHappened();
        }        
        
        [Test]
        public void Then_we_should_request_the_datetimes_each_of_these_backups_was_initiated()
        {
            //Given:
            var possibleFileNames = new[] { "file1", "file2", "file3" };
            A.CallTo(() => _fileSystem.GetFileNames(A<string>._)).Returns(possibleFileNames);

            //When:
            whenABackupIsExpected(new BackupSettings());

            //Then:
            A.CallTo(() => _backupFileDatesQuery.GetDates(possibleFileNames)).MustHaveHappened();
        }
        

        [Test]
        public void Then_we_should_get_a_valid_record_if_a_datetime_occured_within_the_last_2_hours() 
        {
            //Given:
            var possibleFileDates = new[]
                {
                    new BackupFileWithDateStamp("", new DateTime(2011, 12, 10, 12, 12, 45)),
                    new BackupFileWithDateStamp("", new DateTime(2011, 11, 10, 06, 45, 34)),
                    new BackupFileWithDateStamp("", new DateTime(2011, 12, 10, 04, 09, 01))
                };
            A.CallTo(() => _currentDateProvider.Now).Returns(new DateTime(2011, 12, 10, 13, 49, 12));
            A.CallTo(() => _backupFileDatesQuery.GetDates(A<IEnumerable<string>>._)).Returns(possibleFileDates);

            //When:
            var validationRecord = whenABackupIsExpected(new BackupSettings());

            //Then:
            Assert.That(validationRecord.IsValid, Is.True);
        }
        
        [Test]
        public void Then_we_should_get_an_invalid_record_if_a_datetime_did_not_occur_within_the_last_2_hours() 
        {
            //Given:
            var possibleFileDates = new[]
                {
                    new BackupFileWithDateStamp("", new DateTime(2011, 12, 10, 11, 12, 45)),
                    new BackupFileWithDateStamp("", new DateTime(2011, 11, 10, 06, 45, 34)),
                    new BackupFileWithDateStamp("", new DateTime(2011, 12, 10, 04, 09, 01))
                };
            A.CallTo(() => _currentDateProvider.Now).Returns(new DateTime(2011, 12, 10, 13, 49, 12));
            A.CallTo(() => _backupFileDatesQuery.GetDates(A<IEnumerable<string>>._)).Returns(possibleFileDates);

            //When:
            var validationRecord = whenABackupIsExpected(new BackupSettings());

            //Then:
            Assert.That(validationRecord.IsValid, Is.False);
            Assert.That(validationRecord.Message, 
                Is.EqualTo("No backup file created within the last two hours was found"));
        }

    }

    public abstract class BackupExistsValidatorTestBase
    {
        protected FileSystem _fileSystem;
        protected BackupFileDatesQuery _backupFileDatesQuery;
        protected CurrentDateProvider _currentDateProvider;

        [SetUp]
        public void Setup()
        {
            _fileSystem = A.Fake<FileSystem>();
            _backupFileDatesQuery = A.Fake<BackupFileDatesQuery>();
            _currentDateProvider = A.Fake<CurrentDateProvider>();
        }

        public BackupExistsValidator GetSUT(BackupSettings backupSettings)
        {
            return new BackupExistsValidator(backupSettings, 
                _fileSystem, _backupFileDatesQuery, _currentDateProvider);
        }

        protected BackupValidationRecord whenABackupIsExpected(BackupSettings backupSettings)
        {
            BackupExistsValidator backupExistsValidator = GetSUT(backupSettings);
            return backupExistsValidator.GetBackupValidation();
        }
    }
}
