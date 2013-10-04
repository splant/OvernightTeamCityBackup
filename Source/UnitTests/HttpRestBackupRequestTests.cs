using System;
using FakeItEasy;
using NUnit.Framework;
using TeamCityBackupTask;

namespace UnitTests
{
    [TestFixture]
    public class Given_specific_backup_URI : HttpRestBackupRequestTestBase
    {
        [Test]
        public void Then_this_is_passed_to_the_HttpGetBackupRequest()
        {
            //Given:
            string backupRequestUri = "http://BackupAddress";
            var backupSettings = new BackupSettings { BackupRequestUri = backupRequestUri };

            HttpRestBackupRequest httpRestBackupRequest = GetSUT(backupSettings);

            //When:
            httpRestBackupRequest.RequestBackup();

            //Then:
            A.CallTo(() => _httpGetBackupRequest.Get(backupRequestUri)).MustHaveHappened();
        }
    }

    [TestFixture]
    public class Given_the_HttpGetBackupRequest_throws_error : HttpRestBackupRequestTestBase
    {
        [Test]
        public void Then_a_BackupFailed_exception_is_thrown_containing_the_message()
        {
            //Given:
            Exception error = new Exception("error content");
            A.CallTo(() => _httpGetBackupRequest.Get(A<string>._)).Throws(error);

            HttpRestBackupRequest httpRestBackupRequest = GetSUT(new BackupSettings());

            //When:
            TestDelegate whenHttpGetBackupRequestThrows = httpRestBackupRequest.RequestBackup;

            //Then:
            Assert.That(whenHttpGetBackupRequestThrows, 
            Throws.InstanceOf<BackupFailed>().With.Message.ContainsSubstring(error.Message));
        }
    }

    public abstract class HttpRestBackupRequestTestBase
    {
        protected HttpGetBackupRequest _httpGetBackupRequest;
        
        [SetUp]
        public void Setup()
        {
            _httpGetBackupRequest = A.Fake<HttpGetBackupRequest>();
        }

        public HttpRestBackupRequest GetSUT(BackupSettings backupSettings)
        {
            return new HttpRestBackupRequest(backupSettings, _httpGetBackupRequest);
        }
    }
}
