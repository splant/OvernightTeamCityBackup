using System;
using FakeItEasy;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class Given_a_backup_is_requested : BackupControllerTestBase
    {
        [Test]
        public void A_backup_process_is_kicked_off()
        {
            //Given:
            BackupController backupController = GetSUT();

            //When:
            backupController.Backup();

            //Then:
            A.CallTo(() => _backupProcess.ExecuteBackup()).MustHaveHappened();
        }
        
        [Test]
        public void The_backup_is_stored_appropriately()
        {
            //Given:
            BackupController backupController = GetSUT();

            //When:
            backupController.Backup();

            //Then:
            A.CallTo(() => _backupStorage.StoreBackup()).MustHaveHappened();
        }
        
        [Test]
        public void The_backup_is_processed_before_being_stored()
        {
            //Given:
            BackupController backupController = GetSUT();

            using (var scope = Fake.CreateScope())
            {
                //When:
                backupController.Backup();

                //Then:
                using (scope.OrderedAssertions())
                {
                    A.CallTo(() => _backupProcess.ExecuteBackup()).MustHaveHappened();
                    A.CallTo(() => _backupStorage.StoreBackup()).MustHaveHappened();
                }
            }
        }
    }

    [TestFixture]
    public class If_a_backup_fails : BackupControllerTestBase
    {
        [Test]
        public void A_notification_is_sent()
        {
            //Given:
            BackupController backupController = GetSUT();

            string failureMessage = "failure message";
            A.CallTo(() => _backupProcess.ExecuteBackup()).Throws(new BackupFailed(failureMessage));

            //When:
            backupController.Backup();

            //Then:
            string expectedMessage = string.Format("Backup failed: {0}", failureMessage);
            A.CallTo(() => _backupNotifier.SendNotification(expectedMessage)).MustHaveHappened();
        }

        [Test]
        public void It_is_not_stored()
        {
            //Given:
            BackupController backupController = GetSUT();
            A.CallTo(() => _backupProcess.ExecuteBackup()).Throws(new BackupFailed(""));

            //When:
            backupController.Backup();

            //Then:
            A.CallTo(() => _backupStorage.StoreBackup()).MustNotHaveHappened();
        }
    }

    [TestFixture]
    public class If_backup_storage_fails : BackupControllerTestBase
    {
        [Test]
        public void A_notification_is_sent()
        {
            //Given:
            BackupController backupController = GetSUT();

            string failureMessage = "failure message";
            A.CallTo(() => _backupStorage.StoreBackup()).Throws(new StorageFailed(failureMessage));

            //When:
            backupController.Backup();

            //Then:
            string expectedMessage = string.Format("Backup storage failed: {0}", failureMessage);
            A.CallTo(() => _backupNotifier.SendNotification(expectedMessage)).MustHaveHappened();
        }
    }

    [TestFixture]
    public class If_backup_successful : BackupControllerTestBase
    {
        [Test]
        public void A_notification_is_sent()
        {
            //Given:
            BackupController backupController = GetSUT();

            //When:
            backupController.Backup();

            //Then:
            A.CallTo(() => _backupNotifier.SendNotification("Backup was a success")).MustHaveHappened();
        }
    }

    public class StorageFailed : Exception
    {
        public StorageFailed(string failureMessage) : base(failureMessage) {}
    }

    public class BackupFailed : Exception 
    {
        public BackupFailed(string failureMessage) : base(failureMessage) {}
    }

    public abstract class BackupControllerTestBase
    {
        protected BackupProcess _backupProcess;
        protected BackupStorage _backupStorage;
        protected BackupNotifier _backupNotifier;

        [SetUp]
        public void Setup()
        {
            _backupProcess = A.Fake<BackupProcess>();
            _backupStorage = A.Fake<BackupStorage>();
            _backupNotifier = A.Fake<BackupNotifier>();
        }

        public BackupController GetSUT()
        {
            return new BackupController(_backupProcess, _backupStorage, _backupNotifier);
        }
    }

    public interface BackupNotifier 
    {
        void SendNotification(string message);
    }

    public interface BackupStorage
    {
        void StoreBackup();
    }

    public interface BackupProcess 
    {
        void ExecuteBackup();
    }

    public class BackupController 
    {
        private readonly BackupProcess _backupProcess;
        private readonly BackupStorage _backupStorage;
        private readonly BackupNotifier _backupNotifier;

        public BackupController(
            BackupProcess backupProcess, BackupStorage backupStorage, BackupNotifier backupNotifier)
        {
            _backupProcess = backupProcess;
            _backupStorage = backupStorage;
            _backupNotifier = backupNotifier;
        }

        public void Backup()
        {
            try
            {
                DoBackup();
            }
            catch (BackupFailed backupFailedException)
            {
                _backupNotifier.SendNotification("Backup failed: " + backupFailedException.Message);
            }
            catch (StorageFailed storageFailedException)
            {
                _backupNotifier.SendNotification("Backup storage failed: " + storageFailedException.Message);
            }
        }

        private void DoBackup()
        {
            _backupProcess.ExecuteBackup();
            _backupStorage.StoreBackup();
            _backupNotifier.SendNotification("Backup was a success");
        }
    }
}
