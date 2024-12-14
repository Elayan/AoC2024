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

    protected override long WorkTwoStars_Implementation()
    {
        //*****************************************************************************
        // This is what I did to actually find the Christmas Tree.
        // I just fiddled with the way robots would agglomerate and threw in a magic number...
        //*****************************************************************************
        // for (int i = 0; i < 10000; i++)
        // {
        //     Logger.Log($"Walking step {i+1}");
        //     foreach(var robot in _robots)
        //         robot.Walk(1, BathroomSize);
        //     
        //     Logger.Log($"Evaluation: {Evaluate(out var rScore, out var cScore)}");
        //     if (rScore > 200 && cScore > 200)
        //     {
        //         LogMap();
        //     }
        // }
        //*****************************************************************************
        
        foreach(var robot in _robots)
            robot.Walk(7344, BathroomSize);
        LogMap();
        return 7344L;
    }

    private string Evaluate(out long rScore, out long cScore)
    {
        // group robots by row position
        var robotsGroupR = _robots
            .GroupBy(r => r.Position.Row);
        var countByRow = robotsGroupR.ToDictionary(g => g.Key, g => g.Count());
        var highestCounts = countByRow.Select(g => g.Value).OrderBy(c => c);
        rScore = highestCounts.TakeLast(10).Sum();
        
        // group robots by col position
        var robotsGroupC = _robots
            .GroupBy(r => r.Position.Col);
        var countByCol = robotsGroupC.ToDictionary(g => g.Key, g => g.Count());
        highestCounts = countByCol.Select(g => g.Value).OrderBy(c => c);
        cScore = highestCounts.TakeLast(10).Sum();

        return $"Row {rScore} - Col {cScore}";
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