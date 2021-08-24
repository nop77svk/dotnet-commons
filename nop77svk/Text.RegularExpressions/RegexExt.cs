namespace NoP77svk.Text.RegularExpressions
{
    using System;
    using System.Text.RegularExpressions;

    public class RegexExt
    {
        public static ValueTuple<string?, string?> SplitSlashedRegexp(string? rxStr)
        {
            if (string.IsNullOrWhiteSpace(rxStr) || !rxStr.StartsWith('/'))
            {
                return new ValueTuple<string?, string?>(rxStr, null);
            }
            else
            {
                int lastSlashIx = rxStr.LastIndexOf('/');
                if (lastSlashIx == 0)
                    throw new ArgumentException($"Supplied regexp \"{rxStr}\" does not end with a slash", rxStr);

                string pureRxStr = rxStr[1..lastSlashIx];
                string modifiers = rxStr[(lastSlashIx + 1)..].ToLower();
                return new ValueTuple<string?, string?>(pureRxStr, modifiers);
            }
        }

        public static RegexOptions ModifiersToOptions(string? modifiers, bool ignoreUnrecognized = true)
        {
            RegexOptions result = RegexOptions.None;
            if (!string.IsNullOrWhiteSpace(modifiers))
            {
                for (int i = 0; i < modifiers.Length; i++)
                {
                    RegexOptions singleOption = modifiers[i] switch
                    {
                        'i' => RegexOptions.IgnoreCase,
                        'I' => RegexOptions.IgnoreCase,
                        'm' => RegexOptions.Multiline,
                        'M' => RegexOptions.Multiline,
                        's' => RegexOptions.Singleline,
                        'S' => RegexOptions.Singleline,
                        _ => ignoreUnrecognized ? RegexOptions.None : throw new ArgumentOutOfRangeException($"Unrecognized regexp modifier \"{modifiers[i]}\"")
                    };
                    result |= singleOption;
                }
            }

            return result;
        }

        public static Regex ParseSlashedRegexp(string? rxStr, RegexOptions options = RegexOptions.None)
        {
            (string? pureRxStr, string? modifiers) = SplitSlashedRegexp(rxStr);
            return new Regex(pureRxStr ?? string.Empty, options | ModifiersToOptions(modifiers));
        }
    }
}
