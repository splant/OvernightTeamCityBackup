using FakeItEasy;
using NUnit.Framework;
using TeamCityBackupTask;

namespace UnitTests
{
    [TestFixture]
    public class Given_we_are_checking_whether_the_backup_has_finished : BackupFinishedValidatorTestBase
    {
        [Test]
        public void Then_we_return_valid_if_we_get_the_status_Idle()
        {
            //Given:
            A.CallTo(() => _httpBackupStatus.GetBackupStatus()).Returns("Idle");
            BackupFinishedValidator backupFinishedValidator = GetSUT();

            //When:
            BackupValidationRecord validationRecord = backupFinishedValidator.GetBackupValidation();

            //Then:
            Assert.That(validationRecord.IsValid, Is.EqualTo(true));
        }
    }

    public abstract class BackupFinishedValidatorTestBase
    {
        protected HttpBackupStatus _httpBackupStatus;
        protected IntervalHandler _intervalHandler;

        [SetUp]
        public void Setup()
        {
            _httpBackupStatus = A.Fake<HttpBackupStatus>();
            _intervalHandler = A.Fake<IntervalHandler>();
        }

        protected BackupFinishedValidator GetSUT(
            string requestedStatus = "", int timeToWait = 100, int interval = 10)
        {
            return new BackupFinishedValidator(timeToWait, interval, _httpBackupStatus, _intervalHandler);
        }
    }
}
