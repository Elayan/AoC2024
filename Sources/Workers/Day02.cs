using AoC2024.Structures;
using AoCTools.Workers;

namespace AoC2024.Workers.Day02
{
    public class RedNoseAnalysis : WorkerBase
    {
        private RedNoseReports _data;
        public override object Data => _data;

        protected override void ProcessDataLines()
        {
            _data = new RedNoseReports(DataLines);
        }

        protected override long WorkOneStar_Implementation()
        {
            return _data.GetValidReportCount();
        }
    }
}
