using AoC2024.Structures.AnglerFish;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2024.Workers.Day15;

public class AnglerFishWarehouse : WorkerBase
{
    private AnglerFishMap _map;
    public override object Data => _map;

    public bool DoubleSize { get; set; } = false;

    protected override void ProcessDataLines()
    {
        if (DoubleSize)
        {
            Logger.Log("Double sizing everything!!");
            for (var i = 0; i < DataLines.Length; i++)
            {
                DataLines[i] = DataLines[i]
                    .Replace("#", "##")
                    .Replace(".", "..")
                    .Replace("O", "[]")
                    .Replace("@", "@.");
            }
        }
        
        var mapData = DataLines.TakeWhile(l => !string.IsNullOrEmpty(l)).ToArray();
        var movesData = DataLines.Skip(mapData.Length + 1).ToArray();
        _map = new AnglerFishMap(mapData, movesData);
    }

    protected override long WorkOneStar_Implementation()
    {
        _map.LetRobotWalk(Logger.ShowAboveSeverity <= SeverityLevel.Low);
        Logger.Log(_map.ToString());
        return _map.GPSCSum;
    }

    protected override long WorkTwoStars_Implementation()
    {
        return WorkOneStar_Implementation();
    }
}