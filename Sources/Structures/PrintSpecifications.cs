using AoCTools.Loggers;
using System.Text;

namespace AoC2024.Structures
{
    public class PrintConstraint
    {
        public PrintConstraint(string line)
        {
            var split = line.Split('|');
            Before = int.Parse(split[0]);
            After = int.Parse(split[1]);
        }

        public int Before { get; private set; }
        public int After { get; private set; }

        public override string ToString()
        {
            return $"{Before}|{After}";
        }
    }

    public class PrintPageSerie
    {
        public PrintPageSerie(string line)
        {
            var split = line.Split(',');
            var list = new List<int>();
            foreach (var s in split)
                list.Add(int.Parse(s));
            Pages = list.ToArray();
        }

        public int[] Pages { get; private set; }

        public override string ToString()
        {
            return string.Join(",", Pages);
        }

        internal long GetMiddlePage() => Pages[Pages.Length / 2];

        public bool Validate(PrintConstraint[] constraints)
        {
            for (int i = 0; i < Pages.Length; i++)
            {
                for (int j = i + 1; j < Pages.Length; j++)
                {
                    for (int c = 0; c < constraints.Length; c++)
                    {
                        if (constraints[c].Before == Pages[j] && constraints[c].After == Pages[i])
                        {
                            Logger.Log($"Page {Pages[i]} should be after {Pages[j]}");
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public void Fix(PrintConstraint[] constraints)
        {
            for (int i = 0; i < Pages.Length; i++)
            {
                for (int j = i + 1; j < Pages.Length; j++)
                {
                    for (int c = 0; c < constraints.Length; c++)
                    {
                        if (constraints[c].Before == Pages[j] && constraints[c].After == Pages[i])
                        {
                            var memo = Pages[i];
                            Pages[i] = Pages[j];
                            Pages[j] = memo;
                            Logger.Log($"Page {Pages[j]} should be after {Pages[i]}, swapping => {this}");
                        }
                    }
                }
            }
        }
    }

    public class PrintSpecifications
    {
        public PrintConstraint[] Constraints { get; private set; }
        public PrintPageSerie[] Series { get; private set; }

        public PrintSpecifications(string[] lines)
        {
            int index = 0;
            var listC = new List<PrintConstraint>();
            for (; !string.IsNullOrWhiteSpace(lines[index]); index++)
            {
                listC.Add(new PrintConstraint(lines[index]));
            }
            Constraints = listC.ToArray();

            index++;
            var listS = new List<PrintPageSerie>();
            for (; index < lines.Length; index++)
            {
                listS.Add(new PrintPageSerie(lines[index]));
            }
            Series = listS.ToArray();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== CONSTRAINTS ===");
            foreach (var c in Constraints)
                sb.AppendLine(c.ToString());
            sb.AppendLine("=== SERIES ===");
            foreach (var s in Series)
                sb.AppendLine(s.ToString());
            return sb.ToString();
        }
    }
}
