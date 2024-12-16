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
    
    public long DistanceTo(Coordinates coords) => Math.Abs(Position.Row - coords.Row) + Math.Abs(Position.Col - coords.Col);

    public ReindeerPath CreatePathFromHere(ReindeerMove move)
    {
        var newPath = new ReindeerPath();
        newPath.Moves = new List<ReindeerMove>(Moves) { move };
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

    public bool ComputeShortestPath(out List<ReindeerMove> moves, out long cost)
    {
        var allPaths = new List<ReindeerPath>();
        allPaths.Add(new ReindeerPath { Position = StartPosition, Direction = CardinalDirection.East });
        var visitedCells = new Dictionary<LabyrinthCell, bool[]>();
        while (allPaths.Any())
        {
            var path = allPaths.First();
            allPaths.RemoveAt(0);

            var curCell = GetCell(path.Position);
            if (curCell.Coordinates.Equals(EndPosition))
            {
                Logger.Log($"Shortest path found: {string.Join(", ", path.Moves)}");
                cost = path.Cost;
                moves = path.Moves;
                return true;
            }

            if (curCell.Content == CellType.Wall)
            {
                Logger.Log($"Wall at {path.Position}");
                continue;
            }

            if (visitedCells.TryGetValue(curCell, out bool[] visited))
            {
                if (visited[(int)path.Direction])
                {
                    Logger.Log("Running in circle!");
                    continue;
                }

                visited[(int)path.Direction] = true;
            }
            else
            {
                visitedCells.Add(curCell, new bool[4]);
                visitedCells[curCell][(int)path.Direction] = true;
            }
            
            
            allPaths.Add(path.CreatePathFromHere(ReindeerMove.Forward));
            allPaths.Add(path.CreatePathFromHere(ReindeerMove.TurnRight));
            allPaths.Add(path.CreatePathFromHere(ReindeerMove.TurnLeft));
            allPaths = allPaths.OrderBy(p => p.Cost).ToList();
        }
        
        Logger.Log("Found nothing, that's bad!");
        cost = -1;
        moves = null;
        return false;
    }
}