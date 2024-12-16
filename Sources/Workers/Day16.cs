using AoC2024.Structures.ReindeerRace;
using AoCTools.Workers;

namespace AoC2024.Workers.Day16;

public class ReindeerRace : WorkerBase
{
    private LabyrinthMap _map;
    public override object Data => _map;

    protected override void ProcessDataLines()
    {
        _map = new LabyrinthMap(DataLines);
    }

    protected override long WorkOneStar_Implementation()
    {
        var path = _map.ComputeShortestPath(out var cost);
        var forwardSteps = path.Count(m => m == ReindeerMove.Forward);
        return cost;
    }
}