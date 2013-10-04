using System;
using FakeItEasy;
using NUnit.Framework;
using TeamCityBackupTask;

namespace UnitTests
{
    [TestFixture]
    public class Given_backup_process_is_executed : ValidatedBackupProcessTestBase
    {
        [SetUp]
        public void GivenBackupAssumedValid()
        {
            A.CallTo(() => _backupValidator.GetBackupValidation()).Returns(BackupValidationRecord.Valid());
        }
        
        [Test]
        public void A_request_is_made()
        {
            //Given:
            ValidatedBackupProcess validatedBackupProcess = GetSUT();

            //When:
            validatedBackupProcess.ExecuteBackup();

            //Then:
            A.CallTo(() => _backupRequest.RequestBackup()).MustHaveHappened();
        }

        [Test]
        public void The_backup_is_validated_after_the_request_has_been_made()
        {
            //Given:
            ValidatedBackupProcess validatedBackupProcess = GetSUT();

            using (var scope = Fake.CreateScope())
            {
                //When:
                validatedBackupProcess.ExecuteBackup();

                //Then:
                using (scope.OrderedAssertions())
                {
                    //Then:
                    A.CallTo(() => _backupRequest.RequestBackup()).MustHaveHappened();
                    A.CallTo(() => _backupValidator.GetBackupValidation()).MustHaveHappened();
                }
            }
        }
    }

    [TestFixture]
    public class Given_the_backup_fails_validation : ValidatedBackupProcessTestBase
    {
        [Test]
        public void Then_a_BackupFailed_exception_is_thrown()
        {
            //Given:
            ValidatedBackupProcess validatedBackupProcess = GetSUT();

            A.CallTo(() => _backupValidator.GetBackupValidation()).Returns(BackupValidationRecord.Invalid(""));

            //When:
            TestDelegate backupThatFailsValidation = validatedBackupProcess.ExecuteBackup;

            //Then:
            Assert.That(backupThatFailsValidation, Throws.InstanceOf<BackupFailed>());
        }
        
        [Test]
        public void Then_the_reason_for_the_failure_is_part_of_the_validation_record()
        {
            //Given:
            ValidatedBackupProcess validatedBackupProcess = GetSUT();

            string failureReason = "validation failure";
            A.CallTo(() => _backupValidator.GetBackupValidation())
             .Returns(BackupValidationRecord.Invalid(failureReason));

            //When:
            TestDelegate backupThatFailsValidation = validatedBackupProcess.ExecuteBackup;

            //Then:
            Assert.That(backupThatFailsValidation, 
            Throws.InstanceOf<BackupFailed>().With.Message.EqualTo(failureReason));
        }
    }

    [TestFixture]
    public class Given_the_backup_passes_validation : ValidatedBackupProcessTestBase
    {
        [Test]
        public void Then_no_exceptions_are_thrown()
        {
            //Given:
            ValidatedBackupProcess validatedBackupProcess = GetSUT();

            A.CallTo(() => _backupValidator.GetBackupValidation()).Returns(BackupValidationRecord.Valid());

            //When:
            TestDelegate backupThatPassesValidation = validatedBackupProcess.ExecuteBackup;

            //Then:
            Assert.That(backupThatPassesValidation, Throws.Nothing);
        }
    }

    public abstract class ValidatedBackupProcessTestBase
    {
        protected BackupRequest _backupRequest;
        protected BackupValidator _backupValidator;
        
        [SetUp]
        public void Setup()
        {
            _backupRequest = A.Fake<BackupRequest>();
            _backupValidator = A.Fake<BackupValidator>();
        }

        public ValidatedBackupProcess GetSUT()
        {
            return new ValidatedBackupProcess(_backupRequest, _backupValidator);
        }
    }
}
