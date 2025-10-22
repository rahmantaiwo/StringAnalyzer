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

            // Compute properties;
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

        // ✅ Get all strings (with optional filters)
        public async Task<IEnumerable<StringResponse>> GetAllStringsAsync(FilterQueryParams filters)
        {
            var records = await _stringRepo.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                records = records.Where(x => x.Value.Contains(filters.Search, StringComparison.OrdinalIgnoreCase));
            }

            if (filters.MinLength.HasValue)
            {
                records = records.Where(x => x.Length >= filters.MinLength);
            }

            if (filters.MaxLength.HasValue)
            {
                records = records.Where(x => x.Length <= filters.MaxLength);
            }

            // Apply sorting (if any)
            if (!string.IsNullOrEmpty(filters.SortBy))
            {
                records = filters.SortBy.ToLower() switch
                {
                    "length" => filters.SortOrder == "desc" ? records.OrderByDescending(x => x.Length) : records.OrderBy(x => x.Length),
                    "createdat" => filters.SortOrder == "desc" ? records.OrderByDescending(x => x.CreatedAt) : records.OrderBy(x => x.CreatedAt),
                    _ => records
                };
            }

            return records.Select(r => new StringResponse(r));
        }

        // ✅ Get string by its value
        public async Task<StringResponse?> GetStringByValueAsync(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be empty.");

            var allRecords = await _stringRepo.GetAllAsync();
            var record = allRecords.FirstOrDefault(x => x.Value.Equals(value, StringComparison.OrdinalIgnoreCase));

            return record is not null ? new StringResponse(record) : null;
        }

        // ✅ Delete a string
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

        // ✅ Filter by natural language (bonus feature)
        public async Task<IEnumerable<StringResponse>> FilterByNaturalLanguageAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return [];

            var allRecords = await _stringRepo.GetAllAsync();
            query = query.ToLower();

            // Example: "find palindrome", "show unique", etc.
            if (query.Contains("palindrome"))
                allRecords = allRecords.Where(r => r.IsPalindrome);

            if (query.Contains("unique"))
                allRecords = allRecords.OrderByDescending(r => r.UniqueCharacters);

            if (query.Contains("recent"))
                allRecords = allRecords.OrderByDescending(r => r.CreatedAt);

            return allRecords.Select(r => new StringResponse(r));
        }
    }
}
