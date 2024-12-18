using System.Text;

namespace AoC2024.Structures;

public class Instruction
{
    public string Name { get; }
    public Func<int, long, int> Operation { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="operation">Parameters: instruction pointer, combo value ; returns instruction pointer</param>
    public Instruction(string name, Func<int, long, int> operation)
    {
        Name = name;
        Operation = operation;
    }
}

public class Computer
{
    private static Computer _instance;
    private static Instruction[] _instructions =
    {
        new("adv", (ptr, combo) =>
        {
            _instance.RegisterA /= (long)Math.Pow(2, _combos[combo].Invoke());
            return ptr + 2;
        }),
        new("bxl", (ptr, literal) =>
        {
            _instance.RegisterB ^= literal;
            return ptr + 2;
        }),
        new("bst", (ptr, combo) =>
        {
            _instance.RegisterB = _combos[combo].Invoke() % 8;
            return ptr + 2;
        }),
        new("jnz", (ptr, literal) =>
        {
            if (_instance.RegisterA == 0)
                return ptr + 2;
            return (int)literal;
        }),
        new("bxc", (ptr, _) =>
        {
            _instance.RegisterB ^= _instance.RegisterC;
            return ptr + 2;
        }),
        new("out", (ptr, combo) =>
        {
            _instance.Out.Add(_combos[combo].Invoke() % 8);
            return ptr + 2;
        }),
        new("bdv", (ptr, combo) =>
        {
            _instance.RegisterB = _instance.RegisterA / (long)Math.Pow(2, _combos[combo].Invoke());
            return ptr + 2;
        }),
        new("cdv", (ptr, combo) =>
        {
            _instance.RegisterC = _instance.RegisterA / (long)Math.Pow(2, _combos[combo].Invoke());
            return ptr + 2;
        }),
    };

    private static Func<long>[] _combos =
    {
        () => 0L,
        () => 1L,
        () => 2L,
        () => 3L,
        () => _instance.RegisterA,
        () => _instance.RegisterB,
        () => _instance.RegisterC,
    };
    
    public long RegisterA { get; private set; }
    public long RegisterB { get; private set; }
    public long RegisterC { get; private set; }
    public List<int> Program { get; } = new();
    public List<long> Out { get; } = new();

    public Computer(string[] lines)
    {
        _instance = this;

        var split = lines[0].Split(':');
        RegisterA = long.Parse(split[1].Trim());
        split = lines[1].Split(':');
        RegisterB = long.Parse(split[1].Trim());
        split = lines[2].Split(':');
        RegisterC = long.Parse(split[1].Trim());
        split = lines[4].Split(':');
        split = split[1].Trim().Split(',');
        foreach (var s in split)
        {
            Program.Add(int.Parse(s));
        }
    }

    public string Execute()
    {
        int ptr = 0;
        while (ptr < Program.Count - 1)
        {
            ptr = _instructions[Program[ptr]].Operation.Invoke(ptr, Program[ptr + 1]);
        }

        return string.Join(",", Out);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== COMPUTER ===");
        sb.AppendLine($"Register A: {RegisterA}");
        sb.AppendLine($"Register B: {RegisterB}");
        sb.AppendLine($"Register C: {RegisterC}");
        sb.AppendLine($"Program: {string.Join(", ", Program)}");
        sb.AppendLine($"Out: {string.Join(", ", Out)}");
        return sb.ToString();
    }
}