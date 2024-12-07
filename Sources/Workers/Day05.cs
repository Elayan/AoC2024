using AoC2024.Structures;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2024.Workers.Day05
{
    public class Printer : WorkerBase
    {
        private PrintSpecifications _specs;
        public override object Data => _specs;

        protected override void ProcessDataLines()
        {
            _specs = new PrintSpecifications(DataLines);
        }

        protected override long WorkOneStar_Implementation()
        {
            var total = 0L;
            foreach (var s in _specs.Series)
            {
                Logger.Log($"Serie {s}");
                if (s.Validate(_specs.Constraints))
                {
                    total += s.GetMiddlePage();
                    Logger.Log($"[✓] Validated => total = {total}");
                }
                else
                {
                    Logger.Log("[x] Invalid.");
                }
            }
            return total;
        }
    }
}
