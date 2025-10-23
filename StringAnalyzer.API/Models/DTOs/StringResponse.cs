using System.Text.Json;

namespace StringAnalyzer.API.Models.DTOs
{
    public class StringResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public StringPropertiesDto Properties { get; set; } = new();
        public DateTimeOffset CreatedAt { get; set; }

        public StringResponse() { }

        public StringResponse(StringRecord record)
        {
            Id = record.Id;
            Value = record.Value;
            CreatedAt = record.CreatedAt;

            var freqMap = string.IsNullOrEmpty(record.CharacterFrequencyMap)
                ? new Dictionary<string, int>()
                : JsonSerializer.Deserialize<Dictionary<string, int>>(record.CharacterFrequencyMap) ?? new Dictionary<string, int>();

            Properties = new StringPropertiesDto
            {
                Length = record.Length,
                IsPalindrome = record.IsPalindrome,
                UniqueCharacters = record.UniqueCharacters,
                WordCount = record.WordCount,
                Sha256Hash = record.Id,
                CharacterFrequencyMap = freqMap
            };
        }
    }
}
