using System.Text;
using AoCTools.Frame.Map.Extensions;
using AoCTools.Frame.TwoDimensions;
using AoCTools.Frame.TwoDimensions.Map.Abstracts;
using AoCTools.Loggers;

namespace AoC2024.Structures;

public class GardenArea
{
    public char Plant { get; set; }
    public GardenPlot[] Plots { get; set; }
    public long Perimeter { get; set; }
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
                var perimeter = 0L;
                while (areaPlotsToProcess.Any())
                {
                    var plot = areaPlotsToProcess.Pop();
                    area.Add(plot);
                    Logger.Log($"Adding plot at {plot.Coordinates}");

                    var localPerimeter = 4;
                    if (InspectInDirection(plot, CardinalDirection.North, ref plotsInGroup, ref areaPlotsToProcess))
                        localPerimeter--;
                    if (InspectInDirection(plot, CardinalDirection.South, ref plotsInGroup, ref areaPlotsToProcess))
                        localPerimeter--;
                    if (InspectInDirection(plot, CardinalDirection.East, ref plotsInGroup, ref areaPlotsToProcess))
                        localPerimeter--;
                    if (InspectInDirection(plot, CardinalDirection.West, ref plotsInGroup, ref areaPlotsToProcess))
                        localPerimeter--;
                    perimeter += localPerimeter;
                }
                
                Areas.Add(new GardenArea { Plant = group.Key, Plots = area.ToArray(), Perimeter = perimeter });
            }
        }
    }

    private bool InspectInDirection(GardenPlot curPlot, CardinalDirection direction, ref List<GardenPlot> eligiblePlots, ref Stack<GardenPlot> plotsInArea)
    {
        var neighborCoords = curPlot.Coordinates + direction.ToCoordinates();
        if (!IsCoordinateInMap(neighborCoords))
            return false;
        
        var neighbor = eligiblePlots.FirstOrDefault(p => p.Coordinates.Equals(neighborCoords));
        if (neighbor == null)
            return GetCell(neighborCoords).Content == curPlot.Content;

        eligiblePlots.Remove(neighbor);
        plotsInArea.Push(neighbor);
        return true;
    }

    public string ToInfoString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== GARDEN AREAS ===");
        foreach (var area in Areas)
        {
            sb.AppendLine($"AREA {area.Plant}: perimeter {area.Perimeter}");
            sb.AppendLine(string.Join(" ", area.Plots.Select(a => a.Coordinates)));
        }
        return sb.ToString();
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