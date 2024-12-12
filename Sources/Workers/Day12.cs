using AoC2024.Structures;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2024.Workers.Day12;

public class GardenFencer : WorkerBase
{
    private GardenMap _map;
    public override object Data => _map;

    protected override void ProcessDataLines()
    {
        _map = new GardenMap(DataLines.Select(l => l.ToArray()).ToArray());
    }

    protected override long WorkOneStar_Implementation()
    {
        _map.FindAreas();
        LogInfos();
        return _map.Areas.Sum(a => a.Fences.Length * a.Plots.Length);
    }

    protected override long WorkTwoStars_Implementation()
    {
        _map.FindAreas();
        _map.ProcessAreaFences();
        LogInfos();
        return _map.Areas.Sum(a => a.Plots.Length * a.FenceSides.Count);
    }

    private void LogInfos()
    {
        if (Logger.ShowAboveSeverity > SeverityLevel.Low)
            return;
        
        Logger.Log(_map.ToInfoString());
    }
}