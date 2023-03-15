using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class DiceRoller
{
    public List<int> VerboseRoll(string dice)
    {
        int amount = 1;
        int mod = 0;
        List<int> results = new List<int>();
        Match match;
        int num;
        List<int> modifiers = new List<int>();

        if (string.IsNullOrEmpty(dice))
        {
            throw new Exception("Missing dice parameter.");
        }

        if (Regex.IsMatch(dice, @"^\s*(\d+)?\s*d\s*(\d+)\s*(.*?)\s*$"))
        {
            match = Regex.Match(dice, @"^\s*(\d+)?\s*d\s*(\d+)\s*(.*?)\s*$");
            if (!string.IsNullOrEmpty(match.Groups[1].Value))
            {
                amount = int.Parse(match.Groups[1].Value);
            }
            if (!string.IsNullOrEmpty(match.Groups[2].Value))
            {
                dice = match.Groups[2].Value;
            }
            if (!string.IsNullOrEmpty(match.Groups[3].Value))
            {
                foreach (Match m in Regex.Matches(match.Groups[3].Value, @""))
                {
                    modifiers.Add(int.Parse(m.Groups[1].Value.Replace(" ", "")));
                }
                mod = modifiers.Sum();
            }
        }
        else
        {
            int.TryParse(dice, out int result);
            dice = result.ToString();
        }

        if (!int.TryParse(dice, out int diceNum))
        {
            return results;
        }

        for (int i = 0; i < amount; i++)
        {
            if (diceNum != 0)
            {
                num = new Random().Next(1, diceNum + 1);
            }
            else
            {
                num = 0;
            }
            results.Add(num);
        }

        results.Sort();
        if (mod != 0)
        {
            results.Add(mod);
        }

        return results;
    }
}
