﻿using AoCTools.Loggers;
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

        [Test]
        public void Day05()
        {
            TestOneStar(new AoC2024.Workers.Day05.Printer(), GetDataPath(5), 5964);
            TestTwoStars(new AoC2024.Workers.Day05.Printer(), GetDataPath(5), 4719);
        }

        [Test]
        public void Day06() // 17s
        {
            TestOneStar(new AoC2024.Workers.Day06.PastPlaceAnalizer(), GetDataPath(6), 4973);
            TestTwoStars(new AoC2024.Workers.Day06.PastPlaceAnalizer(), GetDataPath(6), 1482);
        }

        [Test]
        public void Day07()
        {
            TestOneStar(new AoC2024.Workers.Day07.RopeBridge(), GetDataPath(7), 10741443549536);
            TestTwoStars(new AoC2024.Workers.Day07.RopeBridge(), GetDataPath(7), 500335179214836);
        }

        [Test]
        public void Day08()
        {
            TestOneStar(new AoC2024.Workers.Day08.Antennas(), GetDataPath(8), 329);
            TestTwoStars(new AoC2024.Workers.Day08.Antennas(), GetDataPath(8), 1190);
        }

        [Test]
        public void Day09()
        {
            TestOneStar(new AoC2024.Workers.Day09.Defragmentation(), GetDataPath(9), 6353658451014);
            TestTwoStars(new AoC2024.Workers.Day09.Defragmentation(), GetDataPath(9), 6382582136592);
        }

        [Test]
        public void Day10()
        {
            TestOneStar(new AoC2024.Workers.Day10.TrailFinder(), GetDataPath(10), 611);
            TestTwoStars(new AoC2024.Workers.Day10.TrailFinder(), GetDataPath(10), 1380);
        }

        [Test]
        public void Day11()
        {
            TestOneStar(new AoC2024.Workers.Day11.QuanticStoneBlinker(), GetDataPath(11), 217443);
            TestTwoStars(new AoC2024.Workers.Day11.QuanticStoneBlinker(), GetDataPath(11), 257246536026785);
        }

        [Test]
        public void Day12()
        {
            TestOneStar(new AoC2024.Workers.Day12.GardenFencer(), GetDataPath(12), 1467094);
            TestTwoStars(new AoC2024.Workers.Day12.GardenFencer(), GetDataPath(12), 881182);
        }
    }
}
