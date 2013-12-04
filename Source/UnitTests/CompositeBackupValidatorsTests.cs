using System.Collections.Generic;
using FakeItEasy;
using NUnit.Framework;
using TeamCityBackupTask;

namespace UnitTests
{
    [TestFixture]
    public class Given_we_have_multiple_backup_validators : CompositeBackupValidatorsTestBase
    {
        [Test]
        public void Then_we_call_validate_backup_on_each_of_them()
        {
            //Given:
            SetValidatorToReturnValid(_backupValidator1);
            SetValidatorToReturnValid(_backupValidator2);
            SetValidatorToReturnValid(_backupValidator3);

            var compositeBackupValidators = GetSUT();

            //When:
            compositeBackupValidators.GetBackupValidation();

            //Then:
            A.CallTo(() => _backupValidator1.GetBackupValidation()).MustHaveHappened();
            A.CallTo(() => _backupValidator2.GetBackupValidation()).MustHaveHappened();
            A.CallTo(() => _backupValidator3.GetBackupValidation()).MustHaveHappened();
        }
    }
    
    [TestFixture]
    public class Given_one_of_the_backup_validators_return_an_invalid_result : CompositeBackupValidatorsTestBase
    {
        [Test]
        public void Then_we_return_this_invalid_result()
        {
            //Given:
            var expectedValidationRecord = BackupValidationRecord.Invalid("Invalid result");

            SetValidatorToReturnValid(_backupValidator1);
            SetValidatorToReturnInvalid(_backupValidator2, expectedValidationRecord);
            SetValidatorToReturnValid(_backupValidator3);

            var compositeBackupValidators = GetSUT();

            //When:
            var backupValidationRecord = compositeBackupValidators.GetBackupValidation();

            //Then:
            Assert.That(backupValidationRecord, Is.EqualTo(expectedValidationRecord));
        }
    }
    
    [TestFixture]
    public class Given_all_backup_validators_return_valid_results : CompositeBackupValidatorsTestBase
    {
        [Test]
        public void Then_we_return_an_overall_valid_result()
        {
            //Given:
            SetValidatorToReturnValid(_backupValidator1);
            SetValidatorToReturnValid(_backupValidator2);
            SetValidatorToReturnValid(_backupValidator3);

            var compositeBackupValidators = GetSUT();

            //When:
            var backupValidationRecord = compositeBackupValidators.GetBackupValidation();

            //Then:
            Assert.That(backupValidationRecord.IsValid, Is.True);
        }
    }

    public abstract class CompositeBackupValidatorsTestBase
    {
        protected IEnumerable<BackupValidator> _multipleBackupValidators;

        protected BackupValidator _backupValidator1;
        protected BackupValidator _backupValidator2;
        protected BackupValidator _backupValidator3;

        [SetUp]
        public void Setup()
        {
            _backupValidator1 = A.Fake<BackupValidator>();
            _backupValidator2 = A.Fake<BackupValidator>();
            _backupValidator3 = A.Fake<BackupValidator>();
            
            _multipleBackupValidators = new [] { _backupValidator1, _backupValidator2, _backupValidator3 };
        }

        public CompositeBackupValidators GetSUT()
        {
            return new CompositeBackupValidators(_multipleBackupValidators);
        }

        public void SetValidatorToReturnValid(BackupValidator backupValidator)
        {
            A.CallTo(() => backupValidator.GetBackupValidation()).Returns(BackupValidationRecord.Valid());
        }
        
        public void SetValidatorToReturnInvalid(
            BackupValidator backupValidator, BackupValidationRecord invalidRecord)
        {
            A.CallTo(() => backupValidator.GetBackupValidation()).Returns(invalidRecord);
        }
    }
}
