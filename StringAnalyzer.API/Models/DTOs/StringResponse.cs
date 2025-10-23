using StringAnalyzer.API.Models;
using StringAnalyzer.API.Models.DTOs;
using System.Text.Json;

public class StringResponse
{
    public string Id { get; set; }
    public string Value { get; set; }
    public StringPropertiesDto Properties { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string? Warning { get; set; }

    public StringResponse(StringRecord record)
    {
        Id = record.Id;
        Value = record.Value;
        Properties = new StringPropertiesDto
        {
            Length = record.Length,
            IsPalindrome = record.IsPalindrome,
            UniqueCharacters = record.UniqueCharacters,
            WordCount = record.WordCount,
            Sha256Hash = record.Id,
            CharacterFrequencyMap = JsonSerializer
    .Deserialize<Dictionary<char, int>>(record.CharacterFrequencyMap)!
    .ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value)

        };
        CreatedAt = record.CreatedAt;
    }
}
