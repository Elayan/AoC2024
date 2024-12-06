using AoCTools.Frame.TwoDimensions;
using AoCTools.Frame.TwoDimensions.Map;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2024.Workers.Day04
{
    public class WordSearcher : WorkerBase
    {
        private CharMap _map;
        public override object Data => _map;

        protected override void ProcessDataLines()
        {
            _map = new CharMap(DataLines);
        }

        protected override long WorkOneStar_Implementation()
        {
            long xmasCount = 0L;
            for (int r = 0; r < _map.RowCount; r++)
            {
                for (int c = 0; c < _map.ColCount; c++)
                {
                    xmasCount += FindXmas(r, c);
                }
            }
            return xmasCount;
        }

        private long FindXmas(int r, int c)
        {
            if (_map.GetCell(r, c).Content != 'X')
                return 0;

           return (FindOrientedMas(r, c,  0, -1) ? 1 : 0)  // up
                + (FindOrientedMas(r, c,  1,  1) ? 1 : 0)  // up-right
                + (FindOrientedMas(r, c,  1,  0) ? 1 : 0)  // right
                + (FindOrientedMas(r, c,  1, -1) ? 1 : 0)  // down-right
                + (FindOrientedMas(r, c,  0,  1) ? 1 : 0)  // down
                + (FindOrientedMas(r, c, -1,  1) ? 1 : 0)  // down-left
                + (FindOrientedMas(r, c, -1,  0) ? 1 : 0)  // left
                + (FindOrientedMas(r, c, -1, -1) ? 1 : 0); // up-left
        }

        private bool FindOrientedMas(int startR, int startC, int dirR, int dirC)
        {
            var mPos = new Coordinates(startR + dirR, startC + dirC);
            var aPos = new Coordinates(startR + dirR + dirR, startC + dirC + dirC);
            var sPos = new Coordinates(startR + dirR + dirR + dirR, startC + dirC + dirC + dirC);
            return _map.IsCoordinateInMap(mPos) && _map.IsCoordinateInMap(aPos) && _map.IsCoordinateInMap(sPos)
                && _map.GetCell(mPos).Content == 'M'
                && _map.GetCell(aPos).Content == 'A'
                && _map.GetCell(sPos).Content == 'S';
        }

        protected override long WorkTwoStars_Implementation()
        {
            long xmasCount = 0L;
            for (int r = 0; r < _map.RowCount; r++)
            {
                for (int c = 0; c < _map.ColCount; c++)
                {
                    xmasCount += FindCrossMas(r, c);
                }
            }
            return xmasCount;
        }

        private long FindCrossMas(int r, int c)
        {
            if (_map.GetCell(r, c).Content != 'A')
                return 0;

            Logger.Log($"A found at {r}-{c}");
            return FindDiagonalCrossMS(r, c) ? 1 : 0;
        }

        // actually we shouldn't look for plus-like crosses...
        private bool FindStraightCrossMS(int r, int c)
        {
            var topPos = new Coordinates(r - 1, c);
            var botPos = new Coordinates(r + 1, c);
            var leftPos = new Coordinates(r, c - 1);
            var rightPos = new Coordinates(r, c + 1);
            if (FindCrossMS(topPos, botPos, leftPos, rightPos))
            {
                Logger.Log($"> found Straight Cross");
                return true;
            }
            return false;
        }

        private bool FindDiagonalCrossMS(int r, int c)
        {
            var topLeftPos = new Coordinates(r - 1, c - 1);
            var botRightPos = new Coordinates(r + 1, c + 1);
            var topRight = new Coordinates(r - 1, c + 1);
            var botLeft = new Coordinates(r + 1, c - 1);
            if (FindCrossMS(topLeftPos, botRightPos, topRight, botLeft))
            {
                Logger.Log($"> found Diagonal Cross");
                return true;
            }
            return false;
        }

        // crossing AB and CD
        private bool FindCrossMS(Coordinates A, Coordinates B, Coordinates C, Coordinates D)
        {
            return _map.IsCoordinateInMap(A) && _map.IsCoordinateInMap(B)
                && _map.IsCoordinateInMap(C) && _map.IsCoordinateInMap(D)
                && IsWritingMS(A, B) && IsWritingMS(C, D);
        }

        private bool IsWritingMS(Coordinates A, Coordinates B)
        {
            var cellA = _map.GetCell(A).Content;
            var cellB = _map.GetCell(B).Content;
            return cellA == 'M' && cellB == 'S' || cellA == 'S' && cellB == 'M';
        }
    }
}
