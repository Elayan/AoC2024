using AoCTools.Numbers;

namespace AoC2024.Structures;

public class SimpleQuanticStones
{
    private List<long> _stones;

    public SimpleQuanticStones(string line)
    {
        _stones = line.Split(' ').Select(s => long.Parse(s)).ToList();
    }

    public long StoneCount => _stones.Count;

    public override string ToString()
    {
        return string.Join(" ", _stones);
    }

    public void Blink()
    {
        var oldStones = new List<long>(_stones);
        _stones.Clear();

        foreach (var oldStone in oldStones)
        {
            if (TryRule1(oldStone, out var ruled1Stone))
            {
                _stones.Add(ruled1Stone);
            }
            else if (TryRule2(oldStone, out var ruled2Stones))
            {
                _stones.AddRange(ruled2Stones);
            }
            else
            {
                Rule3(oldStone, out var ruled3Stone);
                _stones.Add(ruled3Stone);
            }
        }
    }

    private bool TryRule1(long stone, out long ruled1Stone)
    {
        ruled1Stone = 1;
        return stone == 0;
    }

    private bool TryRule2(long stone, out List<long> ruled2Stones)
    {
        ruled2Stones = new List<long>();
        
        var stoneStr = stone.ToString();
        if (stoneStr.Length % 2 != 0)
            return false;
        
        var halfLength = stoneStr.Length / 2;
        ruled2Stones.Add(long.Parse(stoneStr.Substring(0, halfLength)));
        ruled2Stones.Add(long.Parse(stoneStr.Substring(halfLength, halfLength)));
        return true;
    }

    private void Rule3(long stone, out long ruled3Stone)
    {
        ruled3Stone = stone * 2024;
    }
}

public class QuanticStone
{
    public long Number { get; private set; }
    public int DigitCount { get; private set; }
    public List<QuanticStone> Blinks { get; private set; } = null;
    private Dictionary<int, long> _knownBlinkCountAtDepth = new Dictionary<int, long>();
    
    public QuanticStone(long number)
    {
        Number = number;
        DigitCount = NonoMath.GetDigitCount(number);
    }

    public bool IsBlinked => Blinks != null;
    
    public void SetBlinks(List<QuanticStone> blinks) => Blinks = blinks;

    public long GetBlinksCount(int depth)
    {
        if (_knownBlinkCountAtDepth.TryGetValue(depth, out var count))
        {
            return count;
        }
        
        if (depth == 0 || Blinks == null || Blinks.Count == 0)
        {
            _knownBlinkCountAtDepth.Add(depth, 1);
            return 1;
        }
        
        var sum = Blinks.Sum(b => b.GetBlinksCount(depth - 1));
        _knownBlinkCountAtDepth.Add(depth, sum);
        return sum;
    }

    public override string ToString()
    {
        return $"{Number}{(IsBlinked ? "*" : "")}";
    }
}

public class QuanticLine
{
    private List<QuanticStone> _line = new();
    private List<QuanticStone> _blinkables = new();
    private List<QuanticStone> _allKnownStones = new();

    public QuanticLine(string line)
    {
        var split = line.Split(' ');
        foreach (var s in split)
        {
            var stone = new QuanticStone(long.Parse(s));
            _line.Add(stone);
            _blinkables.Add(stone);
            _allKnownStones.Add(stone);
        }
    }

    private QuanticStone GetOrCreateStone(long number)
    {
        var stone = _allKnownStones.FirstOrDefault(s => s.Number == number);
        if (stone == null)
        {
            stone = new QuanticStone(number);
            _allKnownStones.Add(stone);
        }
        return stone;
    }

    public void Blink()
    {
        var blinkablesToBlink = new List<QuanticStone>(_blinkables);
        foreach (var blinkable in blinkablesToBlink)
        {
            if (blinkable.IsBlinked)
            {
                _blinkables.Remove(blinkable);
                continue;
            }

            if (blinkable.Number == 0)
            {
                var newStone = GetOrCreateStone(1);
                blinkable.SetBlinks(new List<QuanticStone> { newStone });
                _blinkables.Add(newStone);
            }
            else if (blinkable.DigitCount % 2 == 0)
            {
                NonoMath.SplitDigits(blinkable.Number, blinkable.DigitCount / 2, out var left, out var right);
                var leftStone = GetOrCreateStone(left);
                var rightStone = GetOrCreateStone(right);
                blinkable.SetBlinks(new List<QuanticStone> { leftStone, rightStone });
                _blinkables.Add(leftStone);
                _blinkables.Add(rightStone);
            }
            else
            {
                var newStone = GetOrCreateStone(blinkable.Number * 2024);
                blinkable.SetBlinks(new List<QuanticStone> { newStone });
                _blinkables.Add(newStone);
            }
        }
    }

    public long GetStoneCount(int blinkDepth)
    {
        return _line.Sum(s => s.GetBlinksCount(blinkDepth));
    }

    public long RawStoneCount => _line.Count;

    public override string ToString()
    {
        return string.Join(" ", _line);
    }
}