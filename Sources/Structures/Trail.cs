using System.Text;
using AoCTools.Frame.Map.Extensions;
using AoCTools.Frame.TwoDimensions;
using AoCTools.Frame.TwoDimensions.Map;
using AoCTools.Frame.TwoDimensions.Map.Abstracts;
using AoCTools.Loggers;

namespace AoC2024.Structures;

public class TrailNode : MapCell<char>
{
    public TrailNode(char content, int row, int col) : base(content, row, col)
    {
    }

    public List<TrailNode> PreviousSteps { get; private set; } = new List<TrailNode>();
    public List<TrailNode> NextSteps { get; private set; } = new List<TrailNode>();
    public bool Visited { get; set; }
    private List<TrailNode> _reachableNines = new List<TrailNode>();
    public int Score => _reachableNines.Count;
    public int Rating { get; private set; } = 0;

    public override string ToString()
    {
        return Content.ToString();
    }

    public string ToVisitString()
    {
        return Visited ? Content.ToString() : ".";
    }

    public void ScoreTheTrail(TrailNode nineNode = null)
    {
        if (nineNode == null && Content == '9')
            nineNode = this;

        if (nineNode == null)
            return;
        
        if (!_reachableNines.Contains(nineNode))
            _reachableNines.Add(nineNode);

        foreach (var previousStep in PreviousSteps)
        {
            previousStep.ScoreTheTrail(nineNode);
        }
    }

    public void RateTheTrail(bool isEnd = true)
    {
        if (isEnd && Content != '9')
            return;
        
        Rating++;
        foreach (var previousStep in PreviousSteps)
        {
            previousStep.RateTheTrail(false);
        }
    }
}

public class TrailMap : Map<TrailNode>
{
    private List<TrailNode> _trailStarts;

    public TrailMap(TrailNode[][] mapCells) : base(mapCells)
    { }

    public TrailMap(char[][] charCells) : base(charCells)
    { }

    protected override string LogTitle => "=== TRAILING MAP ===";
    public long TrailScore => _trailStarts?.Sum(trail => trail.Score) ?? 0;
    public long TrailRating => _trailStarts?.Sum(trail => trail.Rating) ?? 0;

    public void FindTrails()
    {
        _trailStarts = AllCells.Where(cell => cell.Content == '0').ToList();
        Logger.Log($"Found {_trailStarts.Count} trail starting points.");
        
        var nodesToWalk = new Stack<TrailNode>(_trailStarts);
        var trailEnds = new List<TrailNode>();
        while (nodesToWalk.Any())
        {
            var node = nodesToWalk.Pop();
            if (node.Visited)
            {
                Logger.Log($"Node {node.Coordinates} have already been seen, ignoring.");
                continue; // already visited
            }
            
            node.Visited = true;

            if (node.Content == '9')
            {
                Logger.Log($"Node {node.Coordinates} is a 9, it won't go further.");
                trailEnds.Add(node);
                continue; // excellent trail won't go further
            }
            
            var nextValue = (char)(node.Content + 1);
            Logger.Log($"Node {node.Coordinates} is a {node.Content}, we're looking for a path to {nextValue}.");
            
            var foundPath = false;
            foundPath |= InspectDirection(node, CardinalDirection.North, nextValue, ref nodesToWalk);
            foundPath |= InspectDirection(node, CardinalDirection.South, nextValue, ref nodesToWalk);
            foundPath |= InspectDirection(node, CardinalDirection.East, nextValue, ref nodesToWalk);
            foundPath |= InspectDirection(node, CardinalDirection.West, nextValue, ref nodesToWalk);

            if (!foundPath)
            {
                Logger.Log("No path found, trail ends here.");
                trailEnds.Add(node);
            }
        }

        foreach (var trailEnd in trailEnds.Distinct())
        {
            Logger.Log($"Scoring and rating for {trailEnd.Coordinates}.");
            trailEnd.ScoreTheTrail();
            trailEnd.RateTheTrail();
        }
    }

    private bool InspectDirection(TrailNode curNode, CardinalDirection direction, char aimValue, ref Stack<TrailNode> nodesToWalk)
    {
        var dirCoordinates = curNode.Coordinates + direction.ToCoordinates();
        if (TryGetCell(dirCoordinates, out var dirCell) // cell exists
            && dirCell.Content == aimValue)                     // content is what we look for
        {
            Logger.Log($"Found path to {direction}");
            dirCell.PreviousSteps.Add(curNode);
            curNode.NextSteps.Add(dirCell);
            nodesToWalk.Push(dirCell);
            return true;
        }

        return false;
    }

    public string ToInfoString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== VISITED TRAIL MAP ===");
        for (var i = 0; i < _trailStarts.Count; i++)
        {
            sb.AppendLine($"Trail #{i+1}: score {_trailStarts[i].Score}, rating {_trailStarts[i].Rating}");
        }
        sb.AppendLine($"(total {_trailStarts.Count} trails)");
        return sb.ToString();
    }

    public string ToTrailString(int trailId)
    {
        var trail = _trailStarts[trailId];
        var sb = new StringBuilder();
        sb.AppendLine($"=== TRAIL #{trailId+1} ===");
        
        var nodes = new Stack<TrailNode>();
        nodes.Push(trail);
        var seen = new List<TrailNode>();
        seen.Add(trail);
        while (nodes.Any())
        {
            var node = nodes.Pop();
            sb.AppendLine($"Node {node.Coordinates} has path at {string.Join(" ", node.NextSteps.Select(s => s.Coordinates))}");
            foreach (var next in node.NextSteps)
            {
                if (!seen.Contains(next))
                {
                    nodes.Push(next);
                    seen.Add(next);
                }
            }
        }
        
        return sb.ToString();
    }
}