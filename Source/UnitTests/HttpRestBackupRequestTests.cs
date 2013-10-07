using System;
using FakeItEasy;
using NUnit.Framework;
using TeamCityBackupTask;

namespace UnitTests
{
    [TestFixture]
    public class Given_specific_backup_settings : HandledBackupRequestTestBase
    {
        [Test]
        public void Then_this_is_passed_to_the_HttpBackupRequest()
        {
            //Given:
            var backupSettings = new BackupSettings();

            HandledBackupRequest handledBackupRequest = GetSUT(backupSettings);

            //When:
            handledBackupRequest.RequestBackup();

            //Then:
            A.CallTo(() => HttpBackupRequest.Request(backupSettings)).MustHaveHappened();
        }
    }
    

    [TestFixture]
    public class Given_the_HandledBackupRequest_throws_error : HandledBackupRequestTestBase
    {
        [Test]
        public void Then_a_BackupFailed_exception_is_thrown_containing_the_message()
        {
            //Given:
            Exception error = new Exception("error content");
            A.CallTo(() => HttpBackupRequest.Request(A<BackupSettings>._)).Throws(error);

            HandledBackupRequest handledBackupRequest = GetSUT(new BackupSettings());

            //When:
            TestDelegate whenHttpBackupRequestThrows = handledBackupRequest.RequestBackup;

            //Then:
            Assert.That(whenHttpBackupRequestThrows, 
            Throws.InstanceOf<BackupFailed>().With.Message.ContainsSubstring(error.Message));
        }
    }

    public abstract class HandledBackupRequestTestBase
    {
        protected HttpBackupRequest HttpBackupRequest;
        
        [SetUp]
        public void Setup()
        {
            HttpBackupRequest = A.Fake<HttpBackupRequest>();
        }

        public HandledBackupRequest GetSUT(BackupSettings backupSettings)
        {
            return new HandledBackupRequest(backupSettings, HttpBackupRequest);
        }
    }
}
