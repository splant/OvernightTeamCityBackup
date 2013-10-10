using FakeItEasy;
using NUnit.Framework;
using TeamCityBackupTask;

namespace UnitTests
{
    [TestFixture]
    public class Given_we_are_checking_whether_the_backup_has_started : BackupStartedValidatorTestBase
    {
        [Test]
        public void Then_we_return_valid_if_we_get_the_status_Running()
        {
            //Given:
            A.CallTo(() => _httpBackupStatus.GetBackupStatus()).Returns("Running");
            BackupStartedValidator backupStartedValidator = GetSUT();

            //When:
            BackupValidationRecord validationRecord = backupStartedValidator.GetBackupValidation();

            //Then:
            Assert.That(validationRecord.IsValid, Is.EqualTo(true));
        }
    }

    public abstract class BackupStartedValidatorTestBase
    {
        protected HttpBackupStatus _httpBackupStatus;
        protected IntervalHandler _intervalHandler;

        [SetUp]
        public void Setup()
        {
            _httpBackupStatus = A.Fake<HttpBackupStatus>();
            _intervalHandler = A.Fake<IntervalHandler>();
        }

        protected BackupStartedValidator GetSUT(
            string requestedStatus = "", int timeToWait = 100, int interval = 10)
        {
            return new BackupStartedValidator(timeToWait, interval, _httpBackupStatus, _intervalHandler);
        }
    }
}
