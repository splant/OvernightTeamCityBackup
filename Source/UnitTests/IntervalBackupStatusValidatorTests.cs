using FakeItEasy;
using NUnit.Framework;
using TeamCityBackupTask;

namespace UnitTests
{
    [TestFixture]
    public class Given_we_have_found_our_requested_status : IntervalBackupStatusValidatorTestBase
    {
        [Test]
        public void Then_we_return_a_valid_backup_record()
        {
            //Given:
            string requestedStatus = "Status";
            IntervalBackupStatusValidator intervalBackupStatusValidator = GetSUT(requestedStatus);

            A.CallTo(() => _httpBackupStatus.GetBackupStatus()).Returns(requestedStatus);

            //When:
            BackupValidationRecord backupValidationRecord = intervalBackupStatusValidator.GetBackupValidation();

            //Then:
            Assert.That(backupValidationRecord.IsValid, Is.True);
        }
    }

    [TestFixture]
    public class Given_the_requested_status_is_not_found_after_the_allowed_period_of_time : IntervalBackupStatusValidatorTestBase
    {
        [Test]
        public void We_checked_for_the_status_the_expected_amount_of_times()
        {
            //Given:
            int interval = 5;
            int timeToWait = 60;
            int expectedStatusChecks = (timeToWait / interval) + 1; //amount of polled checks + initial check

            IntervalBackupStatusValidator intervalBackupStatusValidator = GetSUT("Not found status", 60, 5);

            //When:
            intervalBackupStatusValidator.GetBackupValidation();

            //Then:
            A.CallTo(() => _httpBackupStatus.GetBackupStatus())
             .MustHaveHappened(Repeated.Exactly.Times(expectedStatusChecks));
        }
        
        [Test]
        public void We_polled_the_expected_amount_of_times()
        {
            //Given:
            int interval = 5;
            int timeToWait = 60;
            int expectedPollCount = (timeToWait / interval);

            IntervalBackupStatusValidator intervalBackupStatusValidator = GetSUT("Not found status", 60, 5);

            //When:
            intervalBackupStatusValidator.GetBackupValidation();

            //Then:
            A.CallTo(() => _intervalHandler.WaitInterval(interval))
             .MustHaveHappened(Repeated.Exactly.Times(expectedPollCount));
        }
        
        [Test]
        public void We_return_an_invalid_backup_record()
        {
            //Given:
            string requestedStatus = "RequestedStatus";
            IntervalBackupStatusValidator intervalBackupStatusValidator = GetSUT(requestedStatus, 60, 5);
            A.CallTo(() => _httpBackupStatus.GetBackupStatus()).Returns("Something else");

            //When:
            BackupValidationRecord validationRecord = intervalBackupStatusValidator.GetBackupValidation();

            //Then:
            Assert.That(validationRecord.IsValid, Is.False);
            Assert.That(validationRecord.Message, Contains.Substring(
                string.Format("No backup with requested state: {0} was found", requestedStatus)));
        }
    }

    public abstract class IntervalBackupStatusValidatorTestBase
    {
        protected HttpBackupStatus _httpBackupStatus;
        protected IntervalHandler _intervalHandler;
        
        [SetUp]
        public void Setup()
        {
            _httpBackupStatus = A.Fake<HttpBackupStatus>();
            _intervalHandler = A.Fake<IntervalHandler>();
        }

        protected IntervalBackupStatusValidator GetSUT(
            string requestedStatus = "", int timeToWait = 100, int interval = 10)
        {
            return new IntervalBackupStatusValidator(requestedStatus, timeToWait, interval, _httpBackupStatus, _intervalHandler);
        }
    }
}
