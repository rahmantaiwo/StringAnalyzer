namespace StringAnalyzer.API.Models.DTOs
{
    public class FilterQueryParams
    {
        public string? Search { get; set; }         
        public int? MinLength { get; set; }        
        public int? MaxLength { get; set; }        
        public string? SortBy { get; set; }        
        public string? SortOrder { get; set; }    
        public int? MinWords { get; set; }         
        public int? MaxWords { get; set; }         
    }
}
