using System.Text;
using AoCTools.Frame.Map.Extensions;
using AoCTools.Frame.TwoDimensions;
using AoCTools.Frame.TwoDimensions.Map.Abstracts;
using AoCTools.Loggers;

namespace AoC2024.Structures;

public class GardenAreaFence
{
    public Coordinates Coords { get; set; }
    public CardinalDirection Direction { get; set; }
}

public class GardenArea
{
    public char Plant { get; set; }
    public GardenPlot[] Plots { get; set; }
    public GardenAreaFence[] Fences { get; set; }
    public List<GardenAreaFence[]> FenceSides { get; set; } = new();

    public void ProcessFences()
    {
        Logger.Log($">>>>> Processing fences for area {Plant}");
        var orientedFences = Fences.GroupBy(f => f.Direction);
        foreach (var group in orientedFences)
        {
            Logger.Log($">>> Fences of direction {group.Key}");
            var fencesInGroup = new List<GardenAreaFence>(group);
            while (fencesInGroup.Any())
            {
                var sideFencesToProcess = new Stack<GardenAreaFence>();
                var first = fencesInGroup.First();
                Logger.Log($"> Building side from fence at {first.Coords}");
                sideFencesToProcess.Push(first);
                fencesInGroup.RemoveAt(0);
                
                var side = new List<GardenAreaFence>();
                while (sideFencesToProcess.Any())
                {
                    var fence = sideFencesToProcess.Pop();
                    side.Add(fence);
                    Logger.Log($"Adding fence at {fence.Coords}");

                    InspectFenceInDirection(fence, CardinalDirection.North, ref fencesInGroup, ref sideFencesToProcess);
                    InspectFenceInDirection(fence, CardinalDirection.South, ref fencesInGroup, ref sideFencesToProcess);
                    InspectFenceInDirection(fence, CardinalDirection.East, ref fencesInGroup, ref sideFencesToProcess);
                    InspectFenceInDirection(fence, CardinalDirection.West, ref fencesInGroup, ref sideFencesToProcess);
                }
                
                FenceSides.Add(side.ToArray());
            }
        }
    }

    private void InspectFenceInDirection(GardenAreaFence curFence, CardinalDirection direction, ref List<GardenAreaFence> eligibleFences, ref Stack<GardenAreaFence> fencesInSide)
    {
        var neighborCoords = curFence.Coords + direction.ToCoordinates();
        var neighbor = eligibleFences.FirstOrDefault(f => f.Coords.Equals(neighborCoords));
        if (neighbor == null)
        {
            return;
        }

        eligibleFences.Remove(neighbor);
        fencesInSide.Push(neighbor);
    }
}

public class GardenMap : Map<GardenPlot>
{
    public GardenMap(GardenPlot[][] mapCells) : base(mapCells) { }
    public GardenMap(char[][] charCells) : base(charCells) { }
    
    public List<GardenArea> Areas { get; } = new();

    public void FindAreas()
    {
        var samePlantGroups = AllCells.GroupBy(c => c.Content);
        foreach (var group in samePlantGroups)
        {
            Logger.Log($">>> Plots of plant {group.Key}");
            var plotsInGroup = new List<GardenPlot>(group);
            while (plotsInGroup.Any())
            {
                var areaPlotsToProcess = new Stack<GardenPlot>();
                var first = plotsInGroup.First();
                Logger.Log($"> Building area from plot at {first.Coordinates}");
                areaPlotsToProcess.Push(first);
                plotsInGroup.RemoveAt(0);
                
                var area = new List<GardenPlot>();
                var fences = new List<GardenAreaFence>();
                while (areaPlotsToProcess.Any())
                {
                    var plot = areaPlotsToProcess.Pop();
                    area.Add(plot);
                    Logger.Log($"Adding plot at {plot.Coordinates}");

                    InspectInDirection(plot, CardinalDirection.North, ref plotsInGroup, ref areaPlotsToProcess, ref fences);
                    InspectInDirection(plot, CardinalDirection.South, ref plotsInGroup, ref areaPlotsToProcess, ref fences);
                    InspectInDirection(plot, CardinalDirection.East, ref plotsInGroup, ref areaPlotsToProcess, ref fences);
                    InspectInDirection(plot, CardinalDirection.West, ref plotsInGroup, ref areaPlotsToProcess, ref fences);
                }
                
                Areas.Add(new GardenArea { Plant = group.Key, Plots = area.ToArray(), Fences = fences.ToArray() });
            }
        }
    }

    private void InspectInDirection(GardenPlot curPlot, CardinalDirection direction, ref List<GardenPlot> eligiblePlots,
        ref Stack<GardenPlot> plotsInArea, ref List<GardenAreaFence> fences)
    {
        var neighborCoords = curPlot.Coordinates + direction.ToCoordinates();
        if (!IsCoordinateInMap(neighborCoords))
        {
            fences.Add(new GardenAreaFence { Coords = curPlot.Coordinates, Direction = direction });
            return;
        }
        
        var neighbor = eligiblePlots.FirstOrDefault(p => p.Coordinates.Equals(neighborCoords));
        if (neighbor == null)
        {
            if (GetCell(neighborCoords).Content != curPlot.Content)
                fences.Add(new GardenAreaFence { Coords = curPlot.Coordinates, Direction = direction });
            return;
        }

        eligiblePlots.Remove(neighbor);
        plotsInArea.Push(neighbor);
    }

    public string ToInfoString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== GARDEN AREAS ===");
        foreach (var area in Areas)
        {
            sb.AppendLine($"AREA {area.Plant}: perimeter {area.Fences.Length}, sides {area.FenceSides.Count}");
            sb.AppendLine(string.Join(" ", area.Plots.Select(a => a.Coordinates)));
        }
        return sb.ToString();
    }

    public void ProcessAreaFences()
    {
        foreach (var area in Areas)
        {
            area.ProcessFences();
        }
    }
}

public class GardenPlot : MapCell<char>
{
    public GardenPlot(char content, int row, int col) : base(content, row, col) { }

    public override string ToString()
    {
        return Content.ToString();
    }
}