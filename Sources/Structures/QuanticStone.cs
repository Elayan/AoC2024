namespace AoC2024.Structures;

public class QuanticStones
{
    private List<long> _stones;

    public QuanticStones(string line)
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