using AoC2024.Structures.AnglerFish;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2024.Workers.Day15;

public class AnglerFishWarehouse : WorkerBase
{
    private AnglerFishMap _map;
    public override object Data => _map;

    protected override void ProcessDataLines()
    {
        var mapData = DataLines.TakeWhile(l => !string.IsNullOrEmpty(l)).ToArray();
        var movesData = DataLines.Skip(mapData.Length + 1).ToArray();
        _map = new AnglerFishMap(mapData, movesData);
    }

    protected override long WorkOneStar_Implementation()
    {
        _map.LetRobotWalk();
        Logger.Log(_map.ToString());
        return _map.GPSCSum;
    }
}