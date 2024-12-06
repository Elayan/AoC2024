using AoC2024.Structures;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2024.Workers.Day06
{
    public class PastPlaceAnalizer : WorkerBase
    {
        private PastPlaceMap _map;
        public override object Data => _map;

        protected override void ProcessDataLines()
        {
            _map = new PastPlaceMap(DataLines.Select(l => l.ToArray()).ToArray());
        }

        protected override long WorkOneStar_Implementation()
        {
            _map.LetTheGuardWalk();
            Logger.Log(_map.ToString());
            return _map.AllCells.Count(c => c.IsVisited);
        }
    }
}
