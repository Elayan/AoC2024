using AoCTools.Loggers;
using System.Text;

namespace AoC2024.Structures
{
    public class HistorianLocations
    {
        private List<int> _locationList1 = new List<int>();
        private List<int> _locationList2 = new List<int>();
        private List<int> _distances = new List<int>();

        private int _size;

        public long TotalDistances => _distances.Sum();

        public HistorianLocations(List<int> list1, List<int> list2)
        {
            _locationList1 = list1;
            _locationList1.Sort();

            _locationList2 = list2;
            _locationList2.Sort();

            _size = Math.Min(list1.Count, list2.Count);
            ComputeDistances();
        }

        private void ComputeDistances()
        {
            for (int i = 0; i < _size; i++)
            {
                _distances.Add(Math.Abs(_locationList1[i] - _locationList2[i]));
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== HISTORIAN LOCATIONS ===");
            for (int i = 0; i < _size; i++)
            {
                sb.Append(_locationList1[i]);
                sb.Append("   ");
                sb.Append(_locationList2[i]);
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
