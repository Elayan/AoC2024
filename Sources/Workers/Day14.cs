using System.Text;
using AoC2024.Structures;
using AoCTools.Frame.TwoDimensions;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2024.Workers.Day14;

public class ToiletRobots : WorkerBase
{
    public Coordinates BathroomSize { get; set; }

    private List<ToiletRobot> _robots = new();
    public override object Data => _robots;

    protected override void ProcessDataLines()
    {
        foreach (var line in DataLines)
        {
            _robots.Add(new ToiletRobot(line));
        }
    }

    protected override long WorkOneStar_Implementation()
    {
        LogMap();
        foreach(var robot in _robots)
            robot.Walk(100, BathroomSize);
        LogMap();
        return ComputeMapValue();
    }

    private long ComputeMapValue()
    {
        var quadrantSize = BathroomSize / 2;
        Logger.Log($"Bathroom size {BathroomSize} ; Quadrant size {quadrantSize}");

        var topLeft = ComputeQuadrantValue(Coordinates.Zero, quadrantSize);
        Logger.Log($"Top left corner ({Coordinates.Zero} {quadrantSize}) = {topLeft}");

        var topMidCoord = new Coordinates(0, quadrantSize.Col + 1);
        var rightMidCoord = new Coordinates(quadrantSize.Row, BathroomSize.Col);
        var topRight = ComputeQuadrantValue(topMidCoord, rightMidCoord);
        Logger.Log($"Top right corner ({topMidCoord} {rightMidCoord}) = {topRight}");

        var leftMidCoord = new Coordinates(quadrantSize.Row + 1, 0);
        var botMidCoord = new Coordinates(BathroomSize.Row, quadrantSize.Col);
        var botLeft = ComputeQuadrantValue(leftMidCoord, botMidCoord);
        Logger.Log($"Bottom left corner ({leftMidCoord} {botMidCoord}) = {botLeft}");

        var centerDecal = new Coordinates(quadrantSize.Row + 1, quadrantSize.Col + 1);
        var botRight = ComputeQuadrantValue(centerDecal, BathroomSize);
        Logger.Log($"Bottom right corner ({centerDecal} {BathroomSize}) = {botRight}");
        
        return topLeft * topRight * botLeft * botRight;
    }

    private long ComputeQuadrantValue(Coordinates minCorner, Coordinates maxCorner)
    {
        return _robots.Count(r => 
            r.Position.Row >= minCorner.Row && r.Position.Row < maxCorner.Row
            && r.Position.Col >= minCorner.Col && r.Position.Col < maxCorner.Col);
    }

    private void LogMap()
    {
        if (Logger.ShowAboveSeverity > SeverityLevel.Low)
            return;

        var sb = new StringBuilder();
        sb.AppendLine();
        for (int row = 0; row < BathroomSize.Row; row++)
        {
            for (int col = 0; col < BathroomSize.Col; col++)
            {
                var count = _robots.Count(r => r.Position.Row == row && r.Position.Col == col);
                sb.Append(count == 0 ? "." : count);
            }
            sb.AppendLine();
        }
        Logger.Log(sb.ToString());
    }
}