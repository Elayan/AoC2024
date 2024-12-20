﻿using AoCTools.Loggers;
using AoCTools.Workers;
using NUnit.Framework;
using System.IO;
using AoCTools.Frame.TwoDimensions;

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

        private void TestOneStar(IWorker worker, string dataPath, string expectedResult)
        {
            var work = worker.WorkOneStar_String(dataPath, SeverityLevel.Never);
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
            
            TestTwoStars(new AoC2024.Workers.Day09.Defragmentation(), GetSamplePath(9), 2858);
        }

        [Test]
        public void Sample10()
        {
            TestOneStar(new AoC2024.Workers.Day10.TrailFinder(), GetSamplePath(10), 36);
            TestTwoStars(new AoC2024.Workers.Day10.TrailFinder(), GetSamplePath(10), 81);
        }

        [Test]
        public void Sample11()
        {
            TestOneStar(new AoC2024.Workers.Day11.QuanticStoneBlinker(), GetSamplePath(11), 55312);
            TestTwoStars(new AoC2024.Workers.Day11.QuanticStoneBlinker(), GetSamplePath(11), 65601038650482);
        }

        [Test]
        public void Sample12()
        {
            TestOneStar(new AoC2024.Workers.Day12.GardenFencer(), GetSamplePath(12, "_a"), 140);
            TestOneStar(new AoC2024.Workers.Day12.GardenFencer(), GetSamplePath(12, "_b"), 772);
            TestOneStar(new AoC2024.Workers.Day12.GardenFencer(), GetSamplePath(12), 1930);
            
            TestTwoStars(new AoC2024.Workers.Day12.GardenFencer(), GetSamplePath(12, "_a"), 80);
            TestTwoStars(new AoC2024.Workers.Day12.GardenFencer(), GetSamplePath(12, "_b"), 436);
            TestTwoStars(new AoC2024.Workers.Day12.GardenFencer(), GetSamplePath(12, "_c"), 236);
            TestTwoStars(new AoC2024.Workers.Day12.GardenFencer(), GetSamplePath(12, "_d"), 368);
            TestTwoStars(new AoC2024.Workers.Day12.GardenFencer(), GetSamplePath(12), 1206);
        }

        [Test]
        public void Sample13()
        {
            TestOneStar(new AoC2024.Workers.Day13.ClawMachineHacker(), GetSamplePath(13), 480);
            TestOneStar(new AoC2024.Workers.Day13.ClawMachineHacker(), GetSamplePath(13, "_a"), 261);
        }

        [Test]
        public void Sample14()
        {
            TestOneStar(new AoC2024.Workers.Day14.ToiletRobots { BathroomSize = new Coordinates(7, 11) },
                        GetSamplePath(14), 12);
            TestOneStar(new AoC2024.Workers.Day14.ToiletRobots { BathroomSize = new Coordinates(7, 11) },
                        GetSamplePath(14, "_one"), 0);
        }

        [Test]
        public void Sample15()
        {
            TestOneStar(new AoC2024.Workers.Day15.AnglerFishWarehouse(), GetSamplePath(15, "_a"), 2028);
            TestOneStar(new AoC2024.Workers.Day15.AnglerFishWarehouse(), GetSamplePath(15), 10092);
            
            TestTwoStars(new AoC2024.Workers.Day15.AnglerFishWarehouse { DoubleSize = true },
                GetSamplePath(15, "_b"), 618);
            TestTwoStars(new AoC2024.Workers.Day15.AnglerFishWarehouse { DoubleSize = true },
                GetSamplePath(15), 9021);
        }

        [Test]
        public void Sample16()
        {
            TestOneStar(new AoC2024.Workers.Day16.ReindeerRace(), GetSamplePath(16), 7036);
            TestOneStar(new AoC2024.Workers.Day16.ReindeerRace(), GetSamplePath(16, "_b"), 11048);
            
            TestTwoStars(new AoC2024.Workers.Day16.ReindeerRace(), GetSamplePath(16), 45);
            TestTwoStars(new AoC2024.Workers.Day16.ReindeerRace(), GetSamplePath(16, "_b"), 64);
        }

        [Test]
        public void Sample17()
        {
            TestOneStar(new AoC2024.Workers.Day17.Compiler(), GetSamplePath(17, "_a"), string.Empty); // B = 1
            TestOneStar(new AoC2024.Workers.Day17.Compiler(), GetSamplePath(17, "_b"), "0,1,2");
            TestOneStar(new AoC2024.Workers.Day17.Compiler(), GetSamplePath(17, "_c"), "4,2,5,6,7,7,7,7,3,1,0"); // A = 0
            TestOneStar(new AoC2024.Workers.Day17.Compiler(), GetSamplePath(17, "_d"), string.Empty); // B = 26
            TestOneStar(new AoC2024.Workers.Day17.Compiler(), GetSamplePath(17, "_e"), string.Empty); // B = 44354
            TestOneStar(new AoC2024.Workers.Day17.Compiler(), GetSamplePath(17), "4,6,3,5,6,3,5,2,1,0");
        }
    }
}
