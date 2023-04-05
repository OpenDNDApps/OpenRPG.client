using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace VGDevs
{
    public static class EnumExtensions
    {
        public static string ToFlagString<T>(this T value, string separator = " ") where T : struct, Enum
        {
            if (!typeof(T).IsDefined(typeof(FlagsAttribute), false))
            {
                throw new ArgumentException($"Type '{typeof(T).Name}' is not a flag enum.");
            }

            var values = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            if (!value.Equals(default(T)))
            {
                values.Remove(default(T));
            }

            var flags = values.Where(flag => value.HasFlag(flag)).Select(flag => flag.ToString());
            return flags.Any() ? string.Join(separator, flags) : "Undefined";
        }
    }
}