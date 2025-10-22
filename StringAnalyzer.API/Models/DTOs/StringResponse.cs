using System.Text.Json;

namespace StringAnalyzer.API.Models.DTOs
{
    public class StringResponse
    {
        public string Id { get; set; }                // sha256_hash_value
        public string Value { get; set; }             // analyzed string
        public StringPropertiesDto Properties { get; set; }  // detailed analysis results
        public DateTimeOffset CreatedAt { get; set; } // timestamp

        public StringResponse(StringRecord record)
        {
            Id = record.Id;
            Value = record.Value;
            CreatedAt = record.CreatedAt;

            Properties = new StringPropertiesDto
            {
                Length = record.Length,
                IsPalindrome = record.IsPalindrome,
                UniqueCharacters = record.UniqueCharacters,
                WordCount = record.WordCount,
                Sha256Hash = record.Id,
                CharacterFrequencyMap = JsonSerializer.Deserialize<Dictionary<char, int>>(record.CharacterFrequencyMap)
            };
        }
    }
}
