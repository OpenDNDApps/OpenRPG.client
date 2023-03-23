using System;

namespace ORC
{
    public class OrcDefinitions
    {
    }
    
    [Flags]
    public enum Alignment
    {
        Undefined = 0,
        Lawful = 1 << 1,
        Chaotic = 1 << 2,
        Neutral = 1 << 3,
        Good = 1 << 4,
        Evil = 1 << 5,
    }

    [Flags]
    public enum CreatureSize
    {
        Undefined = 0,
        Diminutive = 1 << 1,
        Tiny = 1 << 2,
        Small = 1 << 3,
        Medium = 1 << 4,
        Large = 1 << 5,
        Huge = 1 << 6,
        Gargantuan = 1 << 7,
        Colossal = 1 << 8
    }
}