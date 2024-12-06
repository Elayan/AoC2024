using AoC2024.Structures;
using AoCTools.Frame.Map.Extensions;
using AoCTools.Loggers;
using AoCTools.Workers;
using System.Linq;
using System.Text;

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

        protected override long WorkTwoStars_Implementation()
        {
            _map.LetTheGuardWalk();
            var guardStart = _map.AllCells.First(c => c.Content == CellType.GuardStart);
            var eligibleCells = _map.AllCells.Where(c => c.IsVisited && c != guardStart).ToList();
            LogEligibleCells(eligibleCells);

            var loopCount = 0L;
            foreach (var eligibleCell in eligibleCells)
            {
                _map.CleanWalk();

                var memoContent = eligibleCell.Content;
                eligibleCell.SetContent(CellType.Block);

                var result = _map.LetTheGuardWalk();
                if (result == PastPlaceMap.GuardStopReason.Looped)
                    loopCount++;

                eligibleCell.SetContent(memoContent);
            }
            return loopCount;
        }

        private static void LogEligibleCells(List<PastPlaceCell> eligibleCells)
        {
            if (Logger.ShowAboveSeverity > SeverityLevel.Low)
                return;

            var sb = new StringBuilder();
            sb.AppendLine("=== ELIGIBLE CELLS ===");
            foreach (var cell in eligibleCells)
            {
                sb.AppendLine(cell.ToString());
            }
            Logger.Log(sb.ToString());
        }
    }
}
