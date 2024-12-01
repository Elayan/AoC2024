using AoCTools.Loggers;
using AoCTools.Workers;
using NUnit.Framework;
using System.IO;

namespace AoC2024_Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class Days
    {
        private string GetDataPath(int day, string append = "")
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "Days", $"day{day:00}{append}");
        }

        private void TestOneStar(IWorker worker, string dataPath, long expectedResult)
        {
            var work = worker.WorkOneStar(dataPath, SeverityLevel.Always);
            Assert.That(work, Is.EqualTo(expectedResult), $"One Star returned {work}, expected {expectedResult}.");
        }

        private void TestTwoStars(IWorker worker, string dataPath, long expectedResult)
        {
            var work = worker.WorkTwoStars(dataPath, SeverityLevel.Always);
            Assert.That(work, Is.EqualTo(expectedResult), $"Two Stars returned {work}, expected {expectedResult}.");
        }

        [Test]
        public void Day01()
        {
            TestOneStar(new AoC2024.Workers.Day01.LocationsComparator(), GetDataPath(1), 2066446);
        }
    }
}
