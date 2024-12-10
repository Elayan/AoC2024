using AoCTools.Loggers;
using AoCTools.Workers;
using NUnit.Framework;
using System.IO;

namespace AoC2024_Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class Samples
    {
        private string GetSamplePath(int day, string append = "")
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "Samples", $"sample{day:00}{append}");
        }

        private void TestOneStar(IWorker worker, string dataPath, long expectedResult)
        {
            var work = worker.WorkOneStar(dataPath, SeverityLevel.Never);
            Assert.That(work, Is.EqualTo(expectedResult), $"One Star returned {work}, expected {expectedResult}.");
        }

        private void TestTwoStars(IWorker worker, string dataPath, long expectedResult)
        {
            var work = worker.WorkTwoStars(dataPath, SeverityLevel.Never);
            Assert.That(work, Is.EqualTo(expectedResult), $"Two Stars returned {work}, expected {expectedResult}.");
        }

        [Test]
        public void Sample01()
        {
            TestOneStar(new AoC2024.Workers.Day01.LocationsComparator(), GetSamplePath(1), 11);
            TestTwoStars(new AoC2024.Workers.Day01.LocationsComparator(), GetSamplePath(1), 31);
        }

        [Test]
        public void Sample02()
        {
            TestOneStar(new AoC2024.Workers.Day02.RedNoseAnalysis(), GetSamplePath(2), 2);
            TestTwoStars(new AoC2024.Workers.Day02.RedNoseAnalysis(), GetSamplePath(2), 9);
        }

        [Test]
        public void Sample03()
        {
            TestOneStar(new AoC2024.Workers.Day03.Multiplicator(), GetSamplePath(3), 322);
            TestTwoStars(new AoC2024.Workers.Day03.Multiplicator(), GetSamplePath(3), 209);
        }

        [Test]
        public void Sample04()
        {
            TestOneStar(new AoC2024.Workers.Day04.WordSearcher(), GetSamplePath(4), 18);
            TestTwoStars(new AoC2024.Workers.Day04.WordSearcher(), GetSamplePath(4), 9);
        }

        [Test]
        public void Sample05()
        {
            TestOneStar(new AoC2024.Workers.Day05.Printer(), GetSamplePath(5), 143);
            TestTwoStars(new AoC2024.Workers.Day05.Printer(), GetSamplePath(5), 123);
        }

        [Test]
        public void Sample06()
        {
            TestOneStar(new AoC2024.Workers.Day06.PastPlaceAnalizer(), GetSamplePath(6), 41);
            TestTwoStars(new AoC2024.Workers.Day06.PastPlaceAnalizer(), GetSamplePath(6), 6);
        }

        [Test]
        public void Sample07()
        {
            TestOneStar(new AoC2024.Workers.Day07.RopeBridge(), GetSamplePath(7), 3749);
            TestTwoStars(new AoC2024.Workers.Day07.RopeBridge(), GetSamplePath(7), 11387);
        }

        [Test]
        public void Sample08()
        {
            TestOneStar(new AoC2024.Workers.Day08.Antennas(), GetSamplePath(8, "_a"), 2);
            TestOneStar(new AoC2024.Workers.Day08.Antennas(), GetSamplePath(8), 14);

            TestTwoStars(new AoC2024.Workers.Day08.Antennas(), GetSamplePath(8, "_b"), 9);
            TestTwoStars(new AoC2024.Workers.Day08.Antennas(), GetSamplePath(8), 34);
        }

        [Test]
        public void Sample09()
        {
            TestOneStar(new AoC2024.Workers.Day09.Defragmentation(), GetSamplePath(9), 1928);
            TestOneStar(new AoC2024.Workers.Day09.Defragmentation(), GetSamplePath(9, "_b"), 11);
            TestOneStar(new AoC2024.Workers.Day09.Defragmentation(), GetSamplePath(9, "_c"), 641243);
            TestOneStar(new AoC2024.Workers.Day09.Defragmentation(), GetSamplePath(9, "_d"), 91);
        }
    }
}
