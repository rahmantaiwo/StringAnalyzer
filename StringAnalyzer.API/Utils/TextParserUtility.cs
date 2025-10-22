using System.Security.Cryptography;
using System.Text;

namespace StringAnalyzer.API.Utils
{
    public class TextParserUtility
    {
        public static class TextAnalysisUtility
        {
            public static bool IsPalindrome(string input)
            {
                var normalized = new string(input.Where(char.IsLetterOrDigit).ToArray()).ToLower();
                return normalized.SequenceEqual(normalized.Reverse());
            }

            public static Dictionary<char, int> GetFrequencyMap(string input)
            {
                return input.GroupBy(c => c)
                            .ToDictionary(g => g.Key, g => g.Count());
            }

            public static string ComputeSha256Hash(string rawData)
            {
                using var sha256 = SHA256.Create();
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}