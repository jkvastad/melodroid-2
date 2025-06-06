namespace Melodroid_2.LCMs;

public class LCM4
{
    public Bit12Int Keys { get; set; } = new(0b10010001);
    public Dictionary<int, int> factorsAtKeys { get; set; } = new()
    {
        [0] = 4,
        [2] = 9,
        [4] = 5,
        [5] = 8,
        [7] = 3,
        [9] = 10,
        [11] = 15,
    };
}

public class LCM8
{
    public Bit12Int Keys { get; set; } = new(0b100010010101);
    public Dictionary<int, int> factorsAtKeys { get; set; } = new()
    {
        [0] = 8,
        [2] = 9,        
        [4] = 10,
        [7] = 12,
        [11] = 15,        
    };
}

public class LCM2
{
    public Bit12Int Keys { get; set; } = new(0b10000001);
    public Dictionary<int, int> factorsAtKeys { get; set; } = new()
    {
        [0] = 2,        
    };
}