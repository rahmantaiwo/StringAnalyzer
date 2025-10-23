using StringAnalyzer.API.Models;
using StringAnalyzer.API.Models.DTOs;
using StringAnalyzer.API.Repositories.IRepository;
using StringAnalyzer.API.Services.IService;
using System.Text.Json;
using static StringAnalyzer.API.Utils.TextParserUtility;

namespace StringAnalyzer.API.Services.Services
{
    public class StringAnalyzerService(IStringRepository _stringRepo) : IStringAnalyzerService
    {
        public async Task<StringResponse> AnalyzeAndSaveStringAsync(CreateStringRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Value))
                throw new ArgumentException("Value field cannot be empty.");

            if (await _stringRepo.ExistsAsync(request.Value))
                throw new InvalidOperationException("String already exists in the system.");

            var value = request.Value.Trim();
            var length = value.Length;
            var isPalindrome = TextAnalysisUtility.IsPalindrome(value);
            var uniqueCharacters = value.ToLower().Distinct().Count();
            var wordCount = value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            var frequencyMap = TextAnalysisUtility.GetFrequencyMap(value);
            var hash = TextAnalysisUtility.ComputeSha256Hash(value);

            var record = new StringRecord
            {
                Id = hash,
                Value = value,
                Length = length,
                IsPalindrome = isPalindrome,
                UniqueCharacters = uniqueCharacters,
                WordCount = wordCount,
                CharacterFrequencyMap = JsonSerializer.Serialize(frequencyMap),
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _stringRepo.AddAsync(record);
            await _stringRepo.SaveChangesAsync();

            return new StringResponse(record);
        }

        public async Task<IEnumerable<StringResponse>> GetAllStringsAsync(FilterQueryParams filters)
        {
            var records = await _stringRepo.GetAllAsync();

            // Search by substring
            if (!string.IsNullOrWhiteSpace(filters.Search))
                records = records.Where(x => x.Value.Contains(filters.Search, StringComparison.OrdinalIgnoreCase));

            // Min/Max length
            if (filters.MinLength.HasValue)
                records = records.Where(x => x.Length >= filters.MinLength.Value);

            if (filters.MaxLength.HasValue)
                records = records.Where(x => x.Length <= filters.MaxLength.Value);

            // Word count filter
            if (filters.MinWords.HasValue)
                records = records.Where(x => x.WordCount >= filters.MinWords.Value);

            if (filters.MaxWords.HasValue)
                records = records.Where(x => x.WordCount <= filters.MaxWords.Value);

            // Sorting
            if (!string.IsNullOrEmpty(filters.SortBy))
            {
                records = filters.SortBy.ToLower() switch
                {
                    "length" => filters.SortOrder == "desc" ? records.OrderByDescending(x => x.Length) : records.OrderBy(x => x.Length),
                    "createdat" => filters.SortOrder == "desc" ? records.OrderByDescending(x => x.CreatedAt) : records.OrderBy(x => x.CreatedAt),
                    "unique" => filters.SortOrder == "desc" ? records.OrderByDescending(x => x.UniqueCharacters) : records.OrderBy(x => x.UniqueCharacters),
                    _ => records
                };
            }

            return records.Select(r => new StringResponse(r));
        }

        public async Task<StringResponse?> GetStringByValueAsync(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be empty.");

            var allRecords = await _stringRepo.GetAllAsync();
            var record = allRecords.FirstOrDefault(x => x.Value.Equals(value, StringComparison.OrdinalIgnoreCase));

            return record is not null ? new StringResponse(record) : null;
        }

        public async Task<bool> DeleteStringAsync(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be empty.");

            var allRecords = await _stringRepo.GetAllAsync();
            var record = allRecords.FirstOrDefault(x => x.Value.Equals(value, StringComparison.OrdinalIgnoreCase));

            if (record is null)
                return false;

            _stringRepo.DeleteAsync(record);
            await _stringRepo.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<StringResponse>> FilterByNaturalLanguageAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Array.Empty<StringResponse>();

            var allRecords = await _stringRepo.GetAllAsync();
            query = query.ToLower();

            // Palindrome filter
            if (query.Contains("palindrome"))
                allRecords = allRecords.Where(r => r.IsPalindrome);

            // Unique characters sorting
            if (query.Contains("unique"))
                allRecords = allRecords.OrderByDescending(r => r.UniqueCharacters);

            // Most recent sorting
            if (query.Contains("recent"))
                allRecords = allRecords.OrderByDescending(r => r.CreatedAt);

            // Word count detection (e.g., "single word palindrome")
            var wordCountMatch = System.Text.RegularExpressions.Regex.Match(query, @"(\d+)\s*word");
            if (wordCountMatch.Success && int.TryParse(wordCountMatch.Groups[1].Value, out int words))
            {
                allRecords = allRecords.Where(r => r.WordCount == words);
            }

            // Contains specific character (e.g., "contains the letter z")
            var charMatch = System.Text.RegularExpressions.Regex.Match(query, @"contains.*letter\s+([a-z])");
            if (charMatch.Success)
            {
                char c = charMatch.Groups[1].Value[0];
                allRecords = allRecords.Where(r => r.Value.Contains(c, StringComparison.OrdinalIgnoreCase));
            }

            // Min/max length parsing (e.g., "longer than 10 characters")
            var minLengthMatch = System.Text.RegularExpressions.Regex.Match(query, @"longer than (\d+)");
            if (minLengthMatch.Success && int.TryParse(minLengthMatch.Groups[1].Value, out int minLen))
                allRecords = allRecords.Where(r => r.Length > minLen);

            var maxLengthMatch = System.Text.RegularExpressions.Regex.Match(query, @"shorter than (\d+)");
            if (maxLengthMatch.Success && int.TryParse(maxLengthMatch.Groups[1].Value, out int maxLen))
                allRecords = allRecords.Where(r => r.Length < maxLen);

            return allRecords.Select(r => new StringResponse(r));
        }
    }
}
