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

        private void TestOneStar(IWorker worker, string dataPath, long expectedResult, SeverityLevel logLevel = SeverityLevel.Always)
        {
            var work = worker.WorkOneStar(dataPath, logLevel);
            Assert.That(work, Is.EqualTo(expectedResult), $"One Star returned {work}, expected {expectedResult}.");
        }

        private void TestTwoStars(IWorker worker, string dataPath, long expectedResult, SeverityLevel logLevel = SeverityLevel.Always)
        {
            var work = worker.WorkTwoStars(dataPath, logLevel);
            Assert.That(work, Is.EqualTo(expectedResult), $"Two Stars returned {work}, expected {expectedResult}.");
        }

        [Test]
        public void Day01()
        {
            TestOneStar(new AoC2024.Workers.Day01.LocationsComparator(), GetDataPath(1), 2066446);
            TestTwoStars(new AoC2024.Workers.Day01.LocationsComparator(), GetDataPath(1), 24931009);
        }

        [Test]
        public void Day02()
        {
            TestOneStar(new AoC2024.Workers.Day02.RedNoseAnalysis(), GetDataPath(2), 383);
            TestTwoStars(new AoC2024.Workers.Day02.RedNoseAnalysis(), GetDataPath(2), 436);
        }

        [Test]
        public void Day02b()
        {
            TestOneStar(new AoC2024.Workers.Day02.RedNoseAnalysis(), GetDataPath(2, "b"), 524);
            TestTwoStars(new AoC2024.Workers.Day02.RedNoseAnalysis(), GetDataPath(2, "b"), 569);
        }

        [Test]
        public void Day03()
        {
            TestOneStar(new AoC2024.Workers.Day03.Multiplicator(), GetDataPath(3), 155955228);
            TestTwoStars(new AoC2024.Workers.Day03.Multiplicator(), GetDataPath(3), 100189366);
        }

        [Test]
        public void Day04()
        {
            TestOneStar(new AoC2024.Workers.Day04.WordSearcher(), GetDataPath(4), 2549);
            TestTwoStars(new AoC2024.Workers.Day04.WordSearcher(), GetDataPath(4), 2003);
        }
    }
}
