using AoC2024.Structures;
using AoCTools.Frame.TwoDimensions;
using AoCTools.Loggers;
using AoCTools.Workers;
using System.Text;

namespace AoC2024.Workers.Day08
{
    public class Antennas : WorkerBase
    {
        private AntennaMap _map;
        public override object Data => _map;

        protected override void ProcessDataLines()
        {
            _map = new AntennaMap(DataLines.Select(l => l.ToArray()).ToArray());
        }

        protected override long WorkOneStar_Implementation()
        {
            var antennasByFrequency = _map.GetAntennasByFrequency();
            LogAntennasByFrequency(antennasByFrequency);

            var antinodes = new List<Coordinates>();
            foreach (var pair in antennasByFrequency)
            {
                var frequency = pair.Key;
                var antennas = pair.Value;
                Logger.Log($"Finding antinodes for {antennas.Length} antennas with frequency {frequency}");

                for (int i = 0; i < antennas.Length; i++)
                {
                    for (int j = i + 1; j < antennas.Length; j++)
                    {
                        var coord1 = antennas[i].Coordinates;
                        var coord2 = antennas[j].Coordinates;

                        var centerRow = (coord1.Row + coord2.Row) / 2.0;
                        var centerCol = (coord1.Col + coord2.Col) / 2.0;

                        var sym1 = new Coordinates(
                            (long)(3.0 * coord1.Row - 2.0 * centerRow),
                            (long)(3.0 * coord1.Col - 2.0 * centerCol));
                        var sym1inmap = _map.IsCoordinateInMap(sym1);
                        var sym2 = new Coordinates(
                            (long)(3.0 * coord2.Row - 2.0 * centerRow),
                            (long)(3.0 * coord2.Col - 2.0 * centerCol));
                        var sym2inmap = _map.IsCoordinateInMap(sym2);

                        Logger.Log($"Antinodes of {coord1} and {coord2} (center ({centerRow}-{centerCol})) are {sym1}{(sym1inmap ? "" : "[out]")} and {sym2}{(sym2inmap ? "" : "[out]")}.");

                        if (sym1inmap)
                            antinodes.Add(sym1);
                        if (sym2inmap)
                            antinodes.Add(sym2);
                    }
                }
            }

            LogMapWithAntinodes(antinodes);
            return antinodes.Distinct().Count();
        }

        private void LogMapWithAntinodes(List<Coordinates> antinodes)
        {
            if (Logger.ShowAboveSeverity > SeverityLevel.Low)
                return;

            Logger.Log(_map.ToStringWithAntinodes(antinodes));
        }

        private void LogAntennasByFrequency(Dictionary<char, AntennaCell[]> antennasByFrequency)
        {
            if (Logger.ShowAboveSeverity > SeverityLevel.Low)
                return;

            var sb = new StringBuilder();
            sb.AppendLine("=== ANTENNAS ===");
            foreach (var p in antennasByFrequency)
            {
                sb.AppendLine($"Found {p.Value.Length} antennas with frequency {p.Key}:");
                sb.AppendLine($"{string.Join(", ", p.Value.Select(a => a.Coordinates))}");
            }
            Logger.Log(sb.ToString());
        }
    }
}
