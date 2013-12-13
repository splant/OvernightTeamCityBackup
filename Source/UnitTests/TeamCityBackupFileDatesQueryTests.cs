using System;
using System.Linq;
using NUnit.Framework;
using TeamCityBackupTask;

namespace UnitTests
{
    [TestFixture]
    public class Given_a_valid_team_city_backup_file_name : TeamCityBackupFileDatesQueryTestBase
    {
        public static object[] FileDateCases = new object[]
            {
                new object[]{ "TeamCity_Backup_20131121_165032.zip", new DateTime(2013, 11, 21, 16, 50, 32) },
                new object[]{ "TeamCity_Backup_20110323_103232.zip", new DateTime(2011, 03, 23, 10, 32, 32) },
                new object[]{ "TeamCity_Backup_20091230_065900.zip", new DateTime(2009, 12, 30, 06, 59, 00) }
            };

        [TestCaseSource("FileDateCases")]
        public void Then_it_is_converted_to_a_date_time_correctly(string fileName, DateTime expectedBackupDate)
        {
            //Given:
            TeamCityBackupFileDatesQuery teamCityBackupFileDatesQuery = GetSUT();

            //When:
            var backupDateTimes = teamCityBackupFileDatesQuery.GetDates(new[] { fileName });

            //Then:
            Assert.That(backupDateTimes.Any(backupFileWithDateStamp => 
                                            backupFileWithDateStamp.BackupDateTime == expectedBackupDate &&
                                            backupFileWithDateStamp.BackupFileName == fileName));
        }
    }
    
    [TestFixture]
    public class Given_a_group_of_valid_and_invalid_team_city_backup_file_names_are_provided : TeamCityBackupFileDatesQueryTestBase
    {
        [Test]
        public void Then_only_the_valid_ones_are_present_in_the_dates_returned()
        {
            //Given:
            string[] fileNames = new[]
                {
                    "TeamCity_Backup_20131121_165032.zip",
                    "TeamCity_Backup_20110323_103232.exe",
                    "TeamCity_Backup_20110323_103232.zip",
                    "TeamCity_20091230_065900.zip",
                    "TeamCity_Backup_20091230_065900.zip",
                    "TeamCity_Backup.zip",
                    ""
                };

            DateTime[] expectedBackupDates = new[]
                {
                    new DateTime(2013, 11, 21, 16, 50, 32),
                    new DateTime(2011, 03, 23, 10, 32, 32),
                    new DateTime(2009, 12, 30, 06, 59, 00)
                };

            TeamCityBackupFileDatesQuery teamCityBackupFileDatesQuery = GetSUT();

            //When:
            var backupDateTimes = teamCityBackupFileDatesQuery.GetDates(fileNames).ToList();

            //Then:
            foreach (var expectedBackupDate in expectedBackupDates)
                Assert.That(backupDateTimes.Any(b => b.BackupDateTime == expectedBackupDate));

            Assert.That(backupDateTimes.Count(), Is.EqualTo(expectedBackupDates.Count()));
        }
    }

    [TestFixture]
    public class Given_the_file_names_include_a_full_path : TeamCityBackupFileDatesQueryTestBase
    {
        [TestCase("C:\\sample\\path\\TeamCity_Backup_20131121_165032.zip")]
        [TestCase("a\\path\\of\\sorts\\TeamCity_Backup_20110323_103232.zip")]
        [TestCase("F:\\TeamCity_Backup_20091230_065900.zip")]
        public void Then_this_path_is_handled_correctly(string fileNameWithFullPaths)
        {
            //Given:

            TeamCityBackupFileDatesQuery teamCityBackupFileDatesQuery = GetSUT();

            //When:
            var backupFileWithDateStamps = teamCityBackupFileDatesQuery.GetDates(new[]{ fileNameWithFullPaths });

            //Then:
            Assert.That(backupFileWithDateStamps.Any(b => b.BackupFileName == fileNameWithFullPaths));
        }
    }
    
    [TestFixture]
    public class Given_an_invalid_team_city_backup_file_name : TeamCityBackupFileDatesQueryTestBase
    {
        public static object[] FileDateCases = new object[]
            {
                new object[]{ "TeamCity_Backup.zip" },
                new object[]{ "TeamCity_Backup_20110323_103232.exe" },
                new object[]{ "TeamCity_20091230_065900.zip" }
            };

        [TestCaseSource("FileDateCases")]
        public void Then_it_is_not_returned_in_the_list_of_present_backup_files(string fileName)
        {
            //Given:
            TeamCityBackupFileDatesQuery teamCityBackupFileDatesQuery = GetSUT();

            //When:
            var backupDateTimes = teamCityBackupFileDatesQuery.GetDates(new[] { fileName });

            //Then:
            Assert.That(!backupDateTimes.Any());
        }
    }
    
    public abstract class TeamCityBackupFileDatesQueryTestBase
    {
        public TeamCityBackupFileDatesQuery GetSUT()
        {
            return new TeamCityBackupFileDatesQuery();
        }
    }
}
