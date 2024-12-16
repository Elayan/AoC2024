using System.Text;
using AoCTools.Frame.Map.Extensions;
using AoCTools.Frame.TwoDimensions;
using AoCTools.Frame.TwoDimensions.Map.Abstracts;
using AoCTools.Loggers;

namespace AoC2024.Structures.ReindeerRace;

public enum ReindeerMove
{
    Forward,
    TurnRight,
    TurnLeft,
}

public class ReindeerPath
{
    public List<ReindeerMove> Moves { get; private set; } = new();
    public Coordinates Position { get; set; }
    public CardinalDirection Direction { get; set; }
    public long Cost { get; set; }
    public List<ReindeerPath> Variants { get; set; } = new();
    public List<Coordinates> VisitedCoordinates { get; set; } = new();

    public void SetCurrentPositionAsVisited()
    {
        if (!VisitedCoordinates.Contains(Position))
            VisitedCoordinates.Add(Position);
    }

    public void AddVariantToVisited(ReindeerPath variant)
    {
        Variants.Add(variant);
        var memoCount = VisitedCoordinates.Count;
        var allVisitedCoords = VisitedCoordinates.ToList();
        allVisitedCoords.AddRange(variant.VisitedCoordinates);
        VisitedCoordinates = allVisitedCoords.Distinct().ToList();
        Logger.Log($"Added {VisitedCoordinates.Count - memoCount} new visited cells ({memoCount} => {VisitedCoordinates.Count})!");
    }

    public ReindeerPath CreatePathFromHere(ReindeerMove move)
    {
        var newPath = new ReindeerPath();
        newPath.Moves = new List<ReindeerMove>(Moves) { move };
        newPath.Variants = new List<ReindeerPath>(Variants);
        newPath.VisitedCoordinates = new List<Coordinates>(VisitedCoordinates);
        switch (move)
        {
            case ReindeerMove.Forward:
                newPath.Direction = Direction;
                newPath.Position = Position + Direction.ToCoordinates();
                newPath.Cost = Cost + 1;
                break;
            case ReindeerMove.TurnRight:
                newPath.Position = Position;
                newPath.Direction = Direction.GetRightTurn();
                newPath.Cost = Cost + 1000;
                break;
            case ReindeerMove.TurnLeft:
                newPath.Position = Position;
                newPath.Direction = Direction.GetLeftTurn();
                newPath.Cost = Cost + 1000;
                break;
        }
        return newPath;
    }
}

public enum CellType
{
    Empty,
    Wall,
    Start,
    End,
}

public class LabyrinthCell : MapCell<CellType>
{
    public LabyrinthCell(char content, int row, int col) : base(CharToCellType(content), row, col)
    { }
    
    public void SetContent(CellType content) => Content = content;

    public static string CellTypeToChar(CellType type)
    {
        switch (type)
        {
            case CellType.Empty: return ".";
            case CellType.Wall: return "#";
            case CellType.Start: return "S";
            case CellType.End: return "E";
        }

        throw new Exception();
    }

    public static CellType CharToCellType(char c)
    {
        switch (c)
        {
            case '.': return CellType.Empty;
            case '#': return CellType.Wall;
            case 'S': return CellType.Start;
            case 'E': return CellType.End;
        }

        throw new Exception();
    }

    public override string ToString()
    {
        return CellTypeToChar(Content);
    }
}

public class LabyrinthMap : Map<LabyrinthCell>
{
    private LabyrinthMap(LabyrinthCell[][] mapCells) : base(mapCells) { }
    private LabyrinthMap(char[][] charCells) : base(charCells) { }

    public LabyrinthMap(string[] lineCells) : this(lineCells.Select(l => l.ToArray()).ToArray())
    {
        var startCell = AllCells.First(c => c.Content == CellType.Start);
        StartPosition = startCell.Coordinates;
        startCell.SetContent(CellType.Empty);
        
        var endCell = AllCells.First(c => c.Content == CellType.End);
        EndPosition = endCell.Coordinates;
        endCell.SetContent(CellType.Empty);
    }
    
    public Coordinates StartPosition { get; }
    public Coordinates EndPosition { get; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine(base.ToString());
        sb.AppendLine($"Start: {StartPosition}");
        sb.AppendLine($"End: {EndPosition}");
        return sb.ToString();
    }

    public long ComputeShortestPathCost()
    {
        var paths = ComputeShortestPaths(true);
        return paths[0].Cost;
    }

    public long ComputeWalkedCellCountOnShortestPaths()
    {
        var paths = ComputeShortestPaths(false);
        var allVisitedCells = new List<Coordinates>();
        foreach (var path in paths)
        {
            allVisitedCells.AddRange(path.VisitedCoordinates);
        }
        return allVisitedCells.Distinct().Count();
    }

    private class VisitedCell
    {
        public Coordinates Coordinates { get; set; }
        public CardinalDirection Direction { get; set; }
        public long Cost { get; set; }
        public List<ReindeerPath> PathsThatVisited { get; set; } = new();
    }

    public List<ReindeerPath> ComputeShortestPaths(bool stopAtFirstFound)
    {
        var allPaths = new List<ReindeerPath>();
        allPaths.Add(new ReindeerPath { Position = StartPosition, Direction = CardinalDirection.East });

        var visitedCells = new List<VisitedCell>();
        var visitedCellsByPath = new Dictionary<ReindeerPath, List<VisitedCell>>();

        var shortestPaths = new List<ReindeerPath>();
        var shortestPathCost = -1L;

        while (allPaths.Any())
        {
            var path = allPaths.First();
            allPaths.RemoveAt(0);
            Logger.Log($"[cost {path.Cost}] At {path.Position}, turned {path.Direction} (already {path.Moves.Count} moves, {path.VisitedCoordinates.Count} visited cells)");
            Logger.Log(string.Join(", ", path.Moves));
            
            if (shortestPathCost > -1 && path.Cost > shortestPathCost)
            {
                Logger.Log($"No more path as cheap as {shortestPathCost}, returning {shortestPaths.Count} paths!");
                return shortestPaths;
            }

            var curCell = GetCell(path.Position);
            if (curCell.Coordinates.Equals(EndPosition))
            {
                if (!shortestPaths.Any())
                {
                    Logger.Log($"First shortest path found! Cost = {path.Cost}");
                    shortestPathCost = path.Cost;
                }
                
                shortestPaths.Add(path);
                Logger.Log($"Short path found: {string.Join(", ", path.Moves)}");
                if (stopAtFirstFound)
                {
                    Logger.Log($"Stopping at first find!");
                    return shortestPaths;
                }
            }

            if (curCell.Content == CellType.Wall)
            {
                Logger.Log($"Wall at {path.Position}");
                continue;
            }

            var alreadyVisitedCell = visitedCells.FirstOrDefault(vc => vc.Coordinates.Equals(path.Position) && vc.Direction == path.Direction);
            if (alreadyVisitedCell != null)
            {
                if (alreadyVisitedCell.Cost <= path.Cost)
                {
                    Logger.Log($"This is a variant of another path, saving.");
                    foreach (var p in alreadyVisitedCell.PathsThatVisited)
                        p.AddVariantToVisited(path);
                    continue;
                }
                
                Logger.Log($"Running in circle!");
                continue;
            }
            
            path.SetCurrentPositionAsVisited();

            alreadyVisitedCell = new VisitedCell { Coordinates = path.Position, Direction = path.Direction, Cost = path.Cost };
            alreadyVisitedCell.PathsThatVisited.Add(path);
            visitedCells.Add(alreadyVisitedCell);
            
            if (!visitedCellsByPath.TryGetValue(path, out var visitedCellsForPath))
            {
                visitedCellsByPath.Add(path, new List<VisitedCell>());
                visitedCellsForPath = visitedCellsByPath[path];
            }
            visitedCellsForPath.Add(alreadyVisitedCell);
            
            Logger.Log($"Go on!");
            foreach (var visitedCell in visitedCellsForPath)
                visitedCell.PathsThatVisited.Remove(path);
            if (GetCell(path.Position + path.Direction.ToCoordinates()).Content != CellType.Wall)
            {
                var newPath = path.CreatePathFromHere(ReindeerMove.Forward);
                foreach (var v in visitedCellsForPath)
                    v.PathsThatVisited.Add(newPath);
                allPaths.Add(newPath);
            }

            if (GetCell(path.Position + path.Direction.GetRightTurn().ToCoordinates()).Content != CellType.Wall)
            {
                var newPath = path.CreatePathFromHere(ReindeerMove.TurnRight);
                foreach (var v in visitedCellsForPath)
                    v.PathsThatVisited.Add(newPath);
                allPaths.Add(newPath);
            }

            if (GetCell(path.Position + path.Direction.GetLeftTurn().ToCoordinates()).Content != CellType.Wall)
            {
                var newPath = path.CreatePathFromHere(ReindeerMove.TurnLeft);
                foreach (var v in visitedCellsForPath)
                    v.PathsThatVisited.Add(newPath);
                allPaths.Add(newPath);
            }

            allPaths = allPaths.OrderBy(p => p.Cost).ToList();
        }
        
        Logger.Log("Found nothing, that's bad!");
        return null;
    }
}