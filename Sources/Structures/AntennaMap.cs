using AoCTools.Frame.TwoDimensions;
using AoCTools.Frame.TwoDimensions.Map.Abstracts;
using System.Text;
using CellType = AoC2024.Structures.Antenna.CellType;

namespace AoC2024.Structures
{
    public class AntennaMap : Map<AntennaCell>
    {
        public AntennaMap(AntennaCell[][] mapCells) : base(mapCells)
        { }

        public AntennaMap(char[][] charCells) : base(charCells)
        { }

        public Dictionary<char, AntennaCell[]> GetAntennasByFrequency()
            => AllCells.Where(c => c.Content == CellType.Antenna)
                       .GroupBy(c => c.Frequency)
                       .ToDictionary(g => g.Key, g => g.ToArray());

        public string ToStringWithAntinodes(List<Coordinates> antinodes)
        {
            var sb = new StringBuilder();
            sb.AppendLine(LogTitle);
            for (int r = 0; r < MapCells.Length; r++)
            {
                AntennaCell[] row = MapCells[r];
                for (int c = 0; c < row.Length; c++)
                {
                    AntennaCell cell = row[c];
                    sb.Append(antinodes.Contains(cell.Coordinates) ? "#" : cell);
                }

                sb.AppendLine();
            }
            return sb.ToString();
        }
    }

    public class AntennaCell : MapCell<CellType>
    {
        public AntennaCell(char content, int row, int col) : base(CharToType(content), row, col)
        { 
            if (Content == CellType.Antenna)
            {
                Frequency = content;
            }
        }

        public char Frequency { get; private set; }

        public override string ToString()
        {
            return TypeToChar(Content, $"{Frequency}");
        }

        private static string TypeToChar(CellType type, string frequency)
        {
            switch (type)
            {
                case CellType.Antenna: return frequency;
                case CellType.Free: return ".";
                default: throw new NotImplementedException($"Unknown {type}.");
            }
        }

        private static CellType CharToType(char c)
        {
            if (c == '.')
                return CellType.Free;

            return CellType.Antenna;
        }
    }

    namespace Antenna
    {
        public enum CellType
        {
            Antenna,
            Free,
        }
    }
}
