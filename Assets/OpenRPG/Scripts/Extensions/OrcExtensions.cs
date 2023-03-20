using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ORC
{
    public static class OrcExtensions
    {
        #region Alignment
        private static readonly Dictionary<string, Alignment> AlignmentsPair = new Dictionary<string, Alignment>(StringComparer.OrdinalIgnoreCase)
        {
            {"L", Alignment.Lawful},
            {"LAWFUL", Alignment.Lawful},
            {"CHAOTIC", Alignment.Chaotic},
            {"C", Alignment.Chaotic},
            {"NEUTRAL", Alignment.Neutral},
            {"N", Alignment.Neutral},
            {"GOOD", Alignment.Good},
            {"G", Alignment.Good},
            {"EVIL", Alignment.Evil},
            {"E", Alignment.Evil}
        };

        public static Alignment AsAlignment(this string alignment)
        {
            return AlignmentsPair.ContainsKey(alignment.Trim().ToUpper()) ? AlignmentsPair[alignment.Trim().ToUpper()] : Alignment.Undefined;
        }

        public static Alignment AsAlignment(this List<string> sizes)
        {
            Alignment alignment = Alignment.Undefined;

            foreach (string s in sizes)
            {
                if (!AlignmentsPair.TryGetValue(s.Trim().ToUpper(), out Alignment value)) 
                    continue;
                
                alignment |= value;
            }

            return alignment;
        }
        #endregion
        
        #region Creature Size
        private static readonly Dictionary<string, CreatureSize> CreatureSizesPair = new Dictionary<string, CreatureSize>(StringComparer.OrdinalIgnoreCase)
        {
            {"D", CreatureSize.Diminutive},
            {"DIMINUTIVE", CreatureSize.Diminutive},
            {"T", CreatureSize.Tiny},
            {"TINY", CreatureSize.Tiny},
            {"S", CreatureSize.Small},
            {"SMALL", CreatureSize.Small},
            {"M", CreatureSize.Medium},
            {"MEDIUM", CreatureSize.Medium},
            {"L", CreatureSize.Large},
            {"LARGE", CreatureSize.Large},
            {"H", CreatureSize.Huge},
            {"HUGE", CreatureSize.Huge},
            {"G", CreatureSize.Gargantuan},
            {"GARGANTUAN", CreatureSize.Gargantuan},
            {"C", CreatureSize.Colossal},
            {"COLOSSAL", CreatureSize.Colossal}
        };

        public static CreatureSize AsCreatureSize(this string sizeString)
        {
            return CreatureSizesPair.ContainsKey(sizeString.Trim().ToUpper()) ? CreatureSizesPair[sizeString.Trim().ToUpper()] : CreatureSize.Undefined;
        }

        public static CreatureSize AsCreatureSize(this List<string> sizes)
        {
            CreatureSize size = CreatureSize.Undefined;

            foreach (string s in sizes)
            {
                if (!CreatureSizesPair.TryGetValue(s.Trim().ToUpper(), out CreatureSize sizeValue)) 
                    continue;
                
                size |= sizeValue;
            }

            return size;
        }
        #endregion
    }
}
