using AoC2024.Structures;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2024.Workers.Day10;

public class TrailFinder : WorkerBase
{
    private TrailMap _map;
    public override object Data => _map;

    protected override void ProcessDataLines()
    {
        _map = new TrailMap(DataLines.Select(l => l.Select(c => c).ToArray()).ToArray());
    }

    protected override long WorkOneStar_Implementation()
    {
        _map.FindTrails();
        LogMap();
        return _map.TrailScore;
    }

    protected override long WorkTwoStars_Implementation()
    {
        _map.FindTrails();
        LogMap();
        return _map.TrailRating;
    }

    private void LogMap()
    {
        if (Logger.ShowAboveSeverity > SeverityLevel.Low)
            return;
        
        Logger.Log(_map.ToInfoString());
        Logger.Log(_map.ToTrailString(6));
    }
}