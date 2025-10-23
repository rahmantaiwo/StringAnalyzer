using Microsoft.AspNetCore.Mvc;
using StringAnalyzer.API.Models.DTOs;
using StringAnalyzer.API.Services.IService;

namespace StringAnalyzer.API.Controllers
{
    [ApiController]
    [Route("strings")]
    public class StringAnalyzerController : ControllerBase
    {
        private readonly IStringAnalyzerService _service;

        public StringAnalyzerController(IStringAnalyzerService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AnalyzeString([FromBody] CreateStringRequest request)
        {
            var result = await _service.AnalyzeAndSaveStringAsync(request);
            return CreatedAtAction(nameof(GetByValue), new { value = request.Value }, result);
        }

        [HttpGet("{value}")]
        public async Task<IActionResult> GetByValue(string value)
        {
            var result = await _service.GetStringByValueAsync(value);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] FilterQueryParams filters)
        {
            var results = await _service.GetAllStringsAsync(filters);
            return Ok(results);
        }

        [HttpGet("filter-by-natural-language")]
        public async Task<IActionResult> FilterByNaturalLanguage([FromQuery] string query)
        {
            var results = await _service.FilterByNaturalLanguageAsync(query);
            return Ok(results);
        }

        [HttpDelete("{value}")]
        public async Task<IActionResult> Delete(string value)
        {
            var success = await _service.DeleteStringAsync(value);
            return success ? NoContent() : NotFound();
        }
    }
}
