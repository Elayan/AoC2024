using AoC2024.Structures;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2024.Workers.Day09
{
    public class Defragmentation : WorkerBase
    {
        private Disk _disk;
        public override object Data => _disk;

        protected override void ProcessDataLines()
        {
            _disk = new Disk(DataLines[0]);
        }

        protected override long WorkOneStar_Implementation()
        {
            LogDisk();
            _disk.Defragment();
            Logger.Log($"Defragmentation done, {_disk.DefragmentedClusters.Count(c => c.Size == 0)} empty clusters", SeverityLevel.Always);
            LogDefragmentedDisk();
            return _disk.GetDefragmentedChecksum();
        }

        private void LogDisk()
        {
            if (Logger.ShowAboveSeverity > SeverityLevel.Low)
                return;
            
            Logger.Log(_disk.ToDevelopedString());
        }

        private void LogDefragmentedDisk()
        {
            if (Logger.ShowAboveSeverity > SeverityLevel.Low)
                return;
            
            Logger.Log(_disk.ToDefragmentedString());
            Logger.Log(_disk.ToDefragmentedDevelopedString());
        }
    }
}