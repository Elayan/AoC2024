using AoC2024.Structures;
using AoCTools.Workers;
using System.Collections.Generic;

namespace AoC2024.Workers.Day01
{
    public class LocationsComparator : WorkerBase
    {
        public override object Data => _locations;
        private HistorianLocations _locations;

        protected override void ProcessDataLines()
        {
            var list1 = new List<int>();
            var list2 = new List<int>();
            foreach (var line in DataLines)
            {
                var split = line.Split(' ');
                list1.Add(int.Parse(split.First()));
                list2.Add(int.Parse(split.Last()));
            }
            _locations = new HistorianLocations(list1, list2);
        }

        protected override long WorkOneStar_Implementation()
        {
            return _locations.GetTotalDistances();
        }

        protected override long WorkTwoStars_Implementation()
        {
            return _locations.GetSimilarityScore();
        }
    }
}
