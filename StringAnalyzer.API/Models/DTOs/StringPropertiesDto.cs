﻿namespace StringAnalyzer.API.Models.DTOs
{
    public class StringPropertiesDto
    {
        public int Length { get; set; }
        public bool IsPalindrome { get; set; }
        public int UniqueCharacters { get; set; }
        public int WordCount { get; set; }
        public string Sha256Hash { get; set; }
        public Dictionary<char, int> CharacterFrequencyMap { get; set; }
    }
}
