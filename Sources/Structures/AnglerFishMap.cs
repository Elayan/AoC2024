using System.Text;
using AoCTools.Frame.Map.Extensions;
using AoCTools.Frame.TwoDimensions;
using AoCTools.Frame.TwoDimensions.Map.Abstracts;
using AoCTools.Loggers;

namespace AoC2024.Structures.AnglerFish;

public enum CellType
{
    Empty,
    Box,
    Wall,
    Robot,
}

public class AnglerFishMapCell : MapCell<CellType>
{
    public AnglerFishMapCell(char content, int row, int col) : base(CharToCellType(content), row, col)
    { }

    public void SetContent(CellType type) => Content = type;
    public long GPSC => Content == CellType.Box ? 100 * Coordinates.Row + Coordinates.Col : 0L;

    private static CellType CharToCellType(char c)
    {
        switch (c)
        {
            case 'O': return CellType.Box;
            case '#': return CellType.Wall;
            case '@': return CellType.Robot;
            default: return CellType.Empty;
        }
    }

    private static string CellTypeToChar(CellType type)
    {
        switch (type)
        {
            case CellType.Box: return "O";
            case CellType.Wall: return "#";
            case CellType.Robot: return "@";
            default: return ".";
        }
    }

    public override string ToString()
    {
        return CellTypeToChar(Content);
    }
}

public class AnglerFishMap : Map<AnglerFishMapCell>
{
    private AnglerFishMap(AnglerFishMapCell[][] mapCells) : base(mapCells) { }
    private AnglerFishMap(char[][] charCells) : base(charCells) { }

    public AnglerFishMap(string[] lineCells, string[] lineMoves)
        : this(lineCells.Select(l => l.ToArray()).ToArray())
    {
        foreach (var line in lineMoves)
        {
            foreach (var c in line)
            {
                Moves.Add(CharToDirection(c));
            }
        }

        var robotCell = AllCells.First(c => c.Content == CellType.Robot);
        RobotPosition = robotCell.Coordinates;
        robotCell.SetContent(CellType.Empty);
    }

    public List<CardinalDirection> Moves { get; } = new();
    public Coordinates RobotPosition { get; private set; }

    protected override string LogTitle => "=== DARK BRAMBLE ===";
    
    public long GPSCSum => AllCells.Sum(cell => cell.GPSC);

    public override string ToString()
    {
        var sb = new StringBuilder();

        var robotCell = GetCell(RobotPosition);
        robotCell.SetContent(CellType.Robot);
        sb.Append(base.ToString());
        robotCell.SetContent(CellType.Empty);
        
        sb.AppendLine("=== ROBOT MOVES ===");
        foreach (var move in Moves)
        {
            sb.Append(DirectionToChar(move));
        }
        return sb.ToString();
    }

    private static CardinalDirection CharToDirection(char c)
    {
        switch (c)
        {
            case '<': return CardinalDirection.West;
            case '>': return CardinalDirection.East;
            case '^': return CardinalDirection.North;
            case 'v': return CardinalDirection.South;
        }
        throw new Exception();
    }

    private static string DirectionToChar(CardinalDirection direction)
    {
        switch (direction)
        {
            case CardinalDirection.North: return "^";
            case CardinalDirection.South: return "v";
            case CardinalDirection.East: return ">";
            case CardinalDirection.West: return "<";
        }
        throw new Exception();
    }

    public void LetRobotWalk()
    {
        Logger.Log($"Starting position {RobotPosition} - {Moves.Count} moves to apply.");
        foreach (var move in Moves)
        {
            var moveCoords = move.ToCoordinates();
            var nextCell = GetCell(RobotPosition + moveCoords);
            if (nextCell.Content == CellType.Wall)
            {
                Logger.Log($"{RobotPosition}[{move}] Won't walk into wall.");
                continue;
            }

            if (nextCell.Content == CellType.Empty)
            {
                Logger.Log($"{RobotPosition}[{move}] Walking to empty cell.");
                RobotPosition = nextCell.Coordinates;
                continue;
            }
            
            Logger.Log($"{RobotPosition}[{move}] Attempting to push boxes...");
            var boxes = new List<AnglerFishMapCell>();
            var endOfBoxLine = nextCell;
            while (endOfBoxLine.Content == CellType.Box)
            {
                boxes.Add(endOfBoxLine);
                endOfBoxLine = GetCell(endOfBoxLine.Coordinates + moveCoords);
            }

            if (endOfBoxLine.Content == CellType.Wall)
            {
                Logger.Log($"...pushing {boxes.Count} boxes against wall, ignoring.");
                continue;
            }
            
            Logger.Log($"...pushing {boxes.Count} boxes.");
            endOfBoxLine.SetContent(CellType.Box);
            nextCell.SetContent(CellType.Empty);
            RobotPosition = nextCell.Coordinates;
        }
    }
}