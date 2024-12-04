using AoCTools.Frame.TwoDimensions;
using AoCTools.Frame.TwoDimensions.Map;
using AoCTools.Loggers;
using AoCTools.Workers;
using System.Text;
using System.Text.RegularExpressions;

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
    }
}
