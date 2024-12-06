using AoCTools.Frame.Map.Extensions;
using AoCTools.Frame.TwoDimensions;
using AoCTools.Frame.TwoDimensions.Map.Abstracts;
using AoCTools.Loggers;

namespace AoC2024.Structures
{
    public class PastPlaceMap : Map<PastPlaceCell>
    {
        public PastPlaceMap(PastPlaceCell[][] mapCells) : base(mapCells)
        { }

        public PastPlaceMap(char[][] charCells) : base(charCells)
        { }

        private Coordinates _guardStart;

        protected override string LogTitle => "=== PAST PLACE MAP ===";

        public enum GuardStopReason
        {
            Exited,
            Looped
        }

        public GuardStopReason LetTheGuardWalk()
        {
            var guardStart = AllCells.First(c => c.Content == CellType.GuardStart).Coordinates;
            Logger.Log($"Guard starts walking at {guardStart}");

            var curPos = guardStart;
            var curCell = GetCell(curPos);
            var curDir = CardinalDirection.North;
            while(true)
            {
                // guard walks on this cell
                curCell.VisitedInDirection[curDir] = true;
                Logger.Log($"Cell at {curPos} visited as {curDir}.");

                // guard looks ahead
                var nextPos = curPos + curDir.ToCoordinates();
                PastPlaceCell nextCell = null;
                if (!IsCoordinateInMap(nextPos))
                {
                    Logger.Log("Guard exited the area!");
                    return GuardStopReason.Exited; // guard exited!
                }

                if (IsCoordinateInMap(nextPos))
                {
                    nextCell = GetCell(nextPos);
                    if (nextCell.VisitedInDirection[curDir])
                    {
                        Logger.Log("End of ronde!");
                        return GuardStopReason.Looped; // end of ronde!
                    }

                    if (nextCell.Content != CellType.Block)
                    {
                        curPos = nextPos;
                        curCell = nextCell;
                        continue; // continue walking that direction
                    }
                }

                // guard hit a block
                Logger.Log($"Impossible to walk on cell at {nextPos}");
                curDir = curDir.GetRightTurn(); // turn right and try again to walk
            }
        }

        internal void CleanWalk()
        {
            foreach (var cell in AllCells)
                cell.CleanVisits();
        }
    }

    public class PastPlaceCell : MapCell<CellType>
    {
        public PastPlaceCell(char content, int row, int col) : base(CharToCellType(content), row, col)
        { }

        public Dictionary<CardinalDirection, bool> VisitedInDirection { get; set; } 
            = new Dictionary<CardinalDirection, bool>
            { 
                { CardinalDirection.North, false },
                { CardinalDirection.South, false },
                { CardinalDirection.West, false },
                { CardinalDirection.East, false },
            };
        public bool IsVisited => VisitCount > 0;
        public int VisitCount => VisitedInDirection.Values.Count(v => v == true);

        public override string ToString()
        {
            return IsVisited ? "X" : CellTypeToChar(Content);
        }

        private static CellType CharToCellType(char c)
        {
            switch(c)
            {
                case '^': return CellType.GuardStart;
                case '#': return CellType.Block;
                default: return CellType.Free;
            }
        }

        private static string CellTypeToChar(CellType type)
        {
            switch (type)
            {
                case CellType.GuardStart: return "^";
                case CellType.Block: return "#";
                default: return ".";
            }
        }

        internal void CleanVisits()
        {
            VisitedInDirection[CardinalDirection.North] = false;
            VisitedInDirection[CardinalDirection.South] = false;
            VisitedInDirection[CardinalDirection.West] = false;
            VisitedInDirection[CardinalDirection.East] = false;
        }

        public void SetContent(CellType type) => Content = type;
    }

    public enum CellType
    {
        Free,
        Block,
        GuardStart
    }
}
