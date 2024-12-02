﻿using AoCTools.Loggers;
using System.Collections.Generic;
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
            {
                Logger.Log(">> [✓] Validated.");
                return true;
            }

            Logger.Log(Environment.NewLine);
            Logger.Log($"===== Validating report #{reportIndex:000}: {string.Join(" ", report)}");
            var list = new List<int>(report);
            if (report[0] > report[1])
            {
                // if report is decreasing, we check the list as increasing from the end
                list.Reverse();
                Logger.Log($"Report is detected as decreasing, reversing: {string.Join(" ", list)}");
            }

            return ValidateList(list);
        }

        private bool ValidateList(List<int> list)
        {
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
            var validList = new List<int>();
            for (int i = 0; i < _reports.Count; i++)
            {
                if (ValidateReportWithProblemDampener(i, _reports[i]))
                    validList.Add(i);
            }
            Logger.Log($"Valid reports: {string.Join(", ", validList)}");
            return validList.Count;
        }

        private bool ValidateReportWithProblemDampener(int reportIndex, List<int> report)
        {
            if (report.Count < 2)
            {
                Logger.Log(">> [✓] Validated.");
                return true;
            }

            Logger.Log(Environment.NewLine);
            Logger.Log($"===== Validating report #{reportIndex:000}: {string.Join(" ", report)}");
            var list = new List<int>(report);
            if (report[0] > report[1])
            {
                // if report is decreasing, we check the list as increasing from the end
                list.Reverse();
                Logger.Log($"Report is detected as decreasing, reversing: {string.Join(" ", list)}");
            }

            Logger.Log($">> Validating full report...");
            if (ValidateList(list))
                return true;

            for (int x = 0; x < list.Count; x++)
            {
                var subList = new List<int>(list);
                subList.RemoveAt(x);

                Logger.Log($">> Validating sub-report #{x + 1:00}/{list.Count:00} : {string.Join(" ", subList)}");
                if (subList[0] > subList[1])
                {
                    // if report is decreasing, we check the list as increasing from the end
                    subList.Reverse();
                    Logger.Log($"Sub-report is detected as decreasing, reversing: {string.Join(" ", subList)}");
                }

                if (ValidateList(subList))
                    return true;
            }

            return false;
        }
    }
}
