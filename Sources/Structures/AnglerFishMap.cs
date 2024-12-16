using System.Text;
using AoCTools.Frame.Map.Extensions;
using AoCTools.Frame.TwoDimensions;
using AoCTools.Frame.TwoDimensions.Map.Abstracts;
using AoCTools.Loggers;

namespace AoC2024.Structures.AnglerFish;

public enum CellType
{
    Empty,
    SmallBox,
    LeftHalfBox,
    RightHalfBox,
    Wall,
    Robot,
}

public class AnglerFishMapCell : MapCell<CellType>
{
    public AnglerFishMapCell(char content, int row, int col) : base(CharToCellType(content), row, col)
    { }

    public void SetContent(CellType type) => Content = type;

    public long GetGPSC()
    {
        if (Content == CellType.SmallBox || Content == CellType.LeftHalfBox)
            return 100 * Coordinates.Row + Coordinates.Col;

        return 0L;
    }

    public bool IsBox => Content == CellType.SmallBox || Content == CellType.LeftHalfBox || Content == CellType.RightHalfBox;

    private static CellType CharToCellType(char c)
    {
        switch (c)
        {
            case 'O': return CellType.SmallBox;
            case '[': return CellType.LeftHalfBox;
            case ']': return CellType.RightHalfBox;
            case '#': return CellType.Wall;
            case '@': return CellType.Robot;
            default: return CellType.Empty;
        }
    }

    private static string CellTypeToChar(CellType type)
    {
        switch (type)
        {
            case CellType.SmallBox: return "O";
            case CellType.LeftHalfBox: return "[";
            case CellType.RightHalfBox: return "]";
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
    
    public long GPSCSum => AllCells.Sum(cell => cell.GetGPSC());

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

    public void LetRobotWalk(bool verbose)
    {
        Logger.Log($"Starting position {RobotPosition} - {Moves.Count} moves to apply.");
        foreach (var move in Moves)
        {
            var moveCoords = move.ToCoordinates();
            var nextCell = GetCell(RobotPosition + moveCoords);
            if (TryEasyStep(nextCell, move))
                continue;
            
            Logger.Log($"{RobotPosition}[{move}] Attempting to push boxes...");
            var boxesToMove = new Stack<AnglerFishMapCell>();
            var boxesToTryPushing = new Stack<AnglerFishMapCell>();
            boxesToTryPushing.Push(nextCell);
            
            var isMoveVertical = move.IsVertical();
            var isStuck = false;
            while (boxesToTryPushing.Any())
            {
                var boxToPush = boxesToTryPushing.Pop();
                if (boxToPush.Content == CellType.Empty)
                {
                    continue;
                }
                if (boxToPush.Content == CellType.Wall)
                {
                    Logger.Log($">>> trying to push wall {boxToPush.Coordinates}, stuck!");
                    isStuck = true;
                    break;
                }
                
                boxesToMove.Push(boxToPush);
                Logger.Log($">>> will move box_{boxToPush} {boxToPush.Coordinates}");

                var nextBox = GetCell(boxToPush.Coordinates + moveCoords);
                if (!boxesToMove.Contains(nextBox) && !boxesToTryPushing.Contains(nextBox))
                {
                    Logger.Log($">>> (next box is {nextBox.Coordinates})");
                    boxesToTryPushing.Push(nextBox);
                }
                
                if (isMoveVertical)
                {
                    Logger.Log(">>> we're pushing vertically, we check if there's another half...");
                    if (!CheckVerticalHalfBoxes(boxToPush, moveCoords, out var otherHalf))
                    {
                        isStuck = true;
                        break;
                    }

                    if (otherHalf != null && !boxesToMove.Contains(otherHalf) && !boxesToTryPushing.Contains(otherHalf))
                    {
                        Logger.Log($">>> adding other half box_{otherHalf} {otherHalf.Coordinates}");
                        boxesToTryPushing.Push(otherHalf);
                    }
                }
            }

            if (isStuck)
            {
                Logger.Log($"...pushing {boxesToMove.Count} boxes against wall, ignoring.");
                continue;
            }
            
            Logger.Log($"...pushing {boxesToMove.Count} boxes.");
            PushBoxes(boxesToMove, moveCoords);
            nextCell.SetContent(CellType.Empty);
            RobotPosition = nextCell.Coordinates;

            if (verbose)
            {
                var robotCell = GetCell(RobotPosition);
                robotCell.SetContent(CellType.Robot);
                Logger.Log(base.ToString());
                robotCell.SetContent(CellType.Empty);
            }
        }
    }

    private bool TryEasyStep(AnglerFishMapCell nextCell, CardinalDirection move)
    {
        if (nextCell.Content == CellType.Wall)
        {
            Logger.Log($"{RobotPosition}[{move}] Won't walk into wall.");
            return true;
        }

        if (nextCell.Content == CellType.Empty)
        {
            Logger.Log($"{RobotPosition}[{move}] Walking to empty cell.");
            RobotPosition = nextCell.Coordinates;
            return true;
        }

        return false;
    }

    private bool CheckVerticalHalfBoxes(AnglerFishMapCell curBoxCell, Coordinates moveCoords, out AnglerFishMapCell cellToDecal)
    {
        cellToDecal = null;
        if (curBoxCell.Content == CellType.LeftHalfBox)
        {
            var rightHalfCell = GetCell(curBoxCell.Coordinates + CardinalDirection.East.ToCoordinates());
            var rightHalfNextCell = GetCell(rightHalfCell.Coordinates + moveCoords);
            if (rightHalfNextCell.Content == CellType.Wall)
            {
                return false;
            }

            cellToDecal = rightHalfCell;
        }
        else if (curBoxCell.Content == CellType.RightHalfBox)
        {
            var leftHalfCell = GetCell(curBoxCell.Coordinates + CardinalDirection.West.ToCoordinates());
            var leftHalfNextCell = GetCell(leftHalfCell.Coordinates + moveCoords);
            if (leftHalfNextCell.Content == CellType.Wall)
            {
                return false;
            }
                    
            cellToDecal = leftHalfCell;
        }
        
        return true;
    }

    private void PushBoxes(Stack<AnglerFishMapCell> boxes, Coordinates moveCoords)
    {
        var memoBoxContent = boxes.ToDictionary(b => b, b => b.Content);
        var previousBoxLocations = boxes.ToList();

        while (boxes.Any())
        {
            var boxCell = boxes.Pop();
            var nextBoxCell = GetCell(boxCell.Coordinates + moveCoords);
            nextBoxCell.SetContent(memoBoxContent[boxCell]);
            previousBoxLocations.Remove(nextBoxCell);
        }

        foreach (var leftSpace in previousBoxLocations)
        {
            leftSpace.SetContent(CellType.Empty);
        }
    }
}