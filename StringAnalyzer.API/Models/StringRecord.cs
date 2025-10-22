namespace StringAnalyzer.API.Models
{
    public class StringRecord
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public int Length { get; set; }
        public bool IsPalindrome { get; set; }
        public int UniqueCharacters { get; set; }
        public int WordCount { get; set; }
        public string CharacterFrequencyMap { get; set; } 
        public DateTimeOffset CreatedAt { get; set; }
    }
}
