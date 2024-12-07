using System.Text;

namespace AoC2024.Structures
{
    public enum OperationAction
    {
        Addition,
        Multiplication,
        Concatenation,
        None
    }

    public class RopeBridgeOperator
    {
        public long Result { get; private set; }
        public long[] Values { get; private set; }
        public OperationAction[] Operation { get; private set; }

        public RopeBridgeOperator(string line)
        {
            var split = line.Split(':');
            Result = long.Parse(split[0]);

            var list = new List<long>();
            foreach (var val in  split[1].Split(' '))
            {
                if (string.IsNullOrEmpty(val))
                    continue;

                list.Add(long.Parse(val));
            }
            Values = list.ToArray();
        }

        public override string ToString()
        {
            return $"{Result}: {string.Join(" ", Values)}";
        }

        public string ToOperationString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < Values.Length; i++)
            {
                if (i > 0)
                {
                    var op = Operation[i - 1];
                    if (op == OperationAction.Addition)
                        sb.Append(" + ");
                    else if (op == OperationAction.Multiplication)
                        sb.Append(" x ");
                    else if (op == OperationAction.Concatenation)
                        sb.Append(" || ");
                }

                sb.Append(Values[i]);
            }
            sb.Append($" = {Result}");
            return sb.ToString();
        }

        public bool ComputeOperation(bool useConcatenation)
        {
            var operation = ComputeOperation_Impl(OperationAction.None, -1L, Values, useConcatenation);
            if (operation == null)
                return false;

            Operation = operation.ToArray();
            return true;
        }

        public List<OperationAction> ComputeOperation_Impl(OperationAction curAction, long curResult, IEnumerable<long> valuesLeft, bool useConcatenation)
        {
            if (curAction != OperationAction.None)
            {
                if (!valuesLeft.Any())
                {
                    if (curResult == Result)
                    {
                        // found a valid operation!
                        return new List<OperationAction> { curAction };
                    }
                    else return null;
                }

                if (curResult > Result)
                {
                    return null; // with only Add and Mul, we can't go back if Result is exceeded
                }
            }

            var nextValue = valuesLeft.First();
            var newValue = ApplyAction(curAction, curResult, nextValue);
            var newValuesLeft = valuesLeft.Skip(1);

            var result = ComputeOperation_Impl(OperationAction.Addition, newValue, newValuesLeft, useConcatenation);
            if (result == null)
            {
                result = ComputeOperation_Impl(OperationAction.Multiplication, newValue, newValuesLeft, useConcatenation);
            }
            if (result == null && useConcatenation)
            {
                result = ComputeOperation_Impl(OperationAction.Concatenation, newValue, newValuesLeft, useConcatenation);
            }

            if (result == null)
                return null; // didn't find any possible operation from here

            // complete valid operation
            if (curAction != OperationAction.None)
                result.Insert(0, curAction);
            return result;
        }

        private long ApplyAction(OperationAction curAction, long leftValue, long rightValue)
        {
            switch (curAction)
            {
                case OperationAction.None:
                    return rightValue;
                case OperationAction.Addition:
                    return leftValue + rightValue;
                case OperationAction.Multiplication:
                    return leftValue * rightValue;
                case OperationAction.Concatenation:
                    return long.Parse($"{leftValue}{rightValue}");
                default:
                    throw new Exception($"Operation not implemented");
            }
        }
    }
}
