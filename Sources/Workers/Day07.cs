using AoC2024.Structures;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2024.Workers.Day07
{
    public class RopeBridge : WorkerBase
    {
        private RopeBridgeOperator[] _operators;
        public override object Data => _operators;

        protected override void ProcessDataLines()
        {
            var list = new List<RopeBridgeOperator>();
            foreach (var line in DataLines)
                list.Add(new RopeBridgeOperator(line));
            _operators = list.ToArray();
        }

        protected override long WorkOneStar_Implementation()
        {
            return Work(false);
        }

        protected override long WorkTwoStars_Implementation()
        {
            return Work(true);
        }

        private long Work(bool useThirdOperator)
        {
            var total = 0L;
            foreach (var op in _operators)
            {
                Logger.Log(op.ToString());
                var operationFound = op.ComputeOperation(useThirdOperator);
                if (!operationFound)
                {
                    Logger.Log("[x] Invalid operator.");
                    continue;
                }

                if (Logger.ShowAboveSeverity <= SeverityLevel.Low)
                    Logger.Log(op.ToOperationString());
                total += op.Result;
                Logger.Log($"[✓] Valid operator => total = {total}");
            }
            return total;
        }
    }
}
