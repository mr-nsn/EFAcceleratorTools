namespace EFAcceleratorTools.Examples.Util.Extensions;

public static class StringExtensions
{
    public static string ToFirstUpper(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        if (input.Length == 1)
            return input.ToUpper();

        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }
}
