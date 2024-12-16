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
        return _map.ComputeShortestPathCost();
    }

    protected override long WorkTwoStars_Implementation()
    {
        return _map.ComputeWalkedCellCountOnShortestPaths();
    }
}