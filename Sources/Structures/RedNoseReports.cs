using AoCTools.Loggers;
using System.Text;

namespace AoC2024.Structures
{
    public class RedNoseReports
    {
        List<List<int>> _reports = new List<List<int>>();

        public RedNoseReports(string[] reportLines)
        {
            foreach (var line in reportLines)
            {
                var split = line.Split(' ');
                var list = new List<int>();
                foreach (var item in split)
                {
                    list.Add(int.Parse(item));
                }
                _reports.Add(list);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== RED NOSE REPORTS ===");
            for (int i = 0; i < _reports.Count; i++)
            {
                sb.Append($"[{i:000}] ");
                sb.AppendLine(string.Join(" ", _reports[i]));
            }
            return sb.ToString();
        }

        public long GetValidReportCount()
        {
            var validList = new List<int>();
            for (int i = 0; i < _reports.Count; i++)
            {
                if (ValidateReport(i, _reports[i]))
                    validList.Add(i);
            }
            Logger.Log($"Valid reports: {string.Join(", ", validList)}");
            return validList.Count;
        }

        private bool ValidateReport(int reportIndex, List<int> report)
        {
            if (report.Count < 2)
                return true;

            Logger.Log(Environment.NewLine);
            Logger.Log($"===== Validating report #{reportIndex:000}: {string.Join(" ", report)}");
            var list = new List<int>(report);
            if (report[0] > report[1])
            {
                // if report is decreasing, we check the list as increasing from the end
                list.Reverse();
                Logger.Log($"Report is detected as decreasing, reversing: {string.Join(" ", list)}");
            }

            for (int i = 0; i < list.Count - 1; i++)
            {
                var difference = list[i + 1] - list[i];
                Logger.Log($"Difference between {list[i]} and {list[i + 1]} = {difference}");
                if (difference <= 0 || difference > 3)
                {
                    Logger.Log($">> [x] Invalidated.");
                    return false; // report is changing direction, or moving too fast
                }
            }

            Logger.Log(">> [✓] Validated.");
            return true;
        }

        internal long GetValidReportCountWithDampener()
        {
            return 0;
        }
    }
}
