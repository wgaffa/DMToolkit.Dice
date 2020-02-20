namespace Wgaffa.DMToolkit.Extensions
{
    public static class StringExtensions
    {
        public static string SurroundWith(this string source, string characters)
        {
            if (characters.Length < 1)
                return source;

            if (characters.Length < 2)
                return $"{characters[0]}{source}{characters[0]}";
            else
                return $"{characters[0]}{source}{characters[1]}";
        }
    }
}
