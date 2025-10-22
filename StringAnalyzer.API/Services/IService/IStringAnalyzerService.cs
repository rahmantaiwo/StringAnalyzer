using StringAnalyzer.API.Models.DTOs;

namespace StringAnalyzer.API.Services.IService
{
    public interface IStringAnalyzerService
    {
        Task<StringResponse> AnalyzeAndSaveStringAsync(CreateStringRequest request);
        Task<StringResponse?> GetStringByValueAsync(string value);
        Task<IEnumerable<StringResponse>> GetAllStringsAsync(FilterQueryParams filters);
        Task<IEnumerable<StringResponse>> FilterByNaturalLanguageAsync(string query);
        Task<bool> DeleteStringAsync(string value);
    }
}
