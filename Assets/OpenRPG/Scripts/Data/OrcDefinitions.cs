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

    [Flags]
    public enum DamageTypes
    {
        Slashing = 1 << 1,
        Piercing = 1 << 2,
        Bludgeoning = 1 << 3,
        Acid = 1 << 4,
        Cold = 1 << 5,
        Fire = 1 << 6,
        Force = 1 << 7,
        Lightning = 1 << 8,
        Necrotic = 1 << 9,
        Poison = 1 << 10,
        Psychic = 1 << 11,
        Radiant = 1 << 12,
        Thunder = 1 << 13,
        Nonlethal = 1 << 14
    }

    [Flags]
    public enum ArmourCategories
    {
        Natural = 1 << 1,
        Light = 1 << 2,
        Medium = 1 << 3,
        Heavy = 1 << 4,
        Shield = 1 << 5
    }

    public struct CreatureAttributes
    {
        public int STR;
        public int DEX;
        public int CON;
        public int INT;
        public int WIS;
        public int CHA;

        public override string ToString()
        {
            return $"STR: {STR} ({ToAbilityBonus(STR)}), DEX: {DEX} ({ToAbilityBonus(DEX)}), CON: {CON} ({ToAbilityBonus(CON)}), INT: {INT} ({ToAbilityBonus(INT)}), WIS: {WIS} ({ToAbilityBonus(WIS)}), CHA: {CHA} ({ToAbilityBonus(CHA)})";
        }
        
        public static string ToAbilityBonus(int abilityScore)
        {
            var bonus = (abilityScore - 10) / 2;
            return (bonus >= 0 ? "+" : "") + bonus.ToString();
        }
    }
}