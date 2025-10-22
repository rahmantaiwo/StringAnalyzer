using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StringAnalyzer.API.Models.DTOs;
using StringAnalyzer.API.Services.IService;

namespace StringAnalyzer.API.Controllers
{
    [Route("strings")]
    [ApiController]
    public class StringsController(IStringAnalyzerService _stringAnalyzerServ) : ControllerBase
    {
       
        [HttpPost]
        public async Task<IActionResult> Analyze([FromBody] CreateStringRequest request)
        {
            var result = await _stringAnalyzerServ.AnalyzeAndSaveStringAsync(request);
            return Ok(result);
        }

        [HttpGet("{value}")]
        public async Task<IActionResult> GetByValue(string value)
        {
            var result = await _stringAnalyzerServ.GetStringByValueAsync(value);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] FilterQueryParams filters)
        {
            var results = await _stringAnalyzerServ.GetAllStringsAsync(filters);
            return Ok(results);
        }

        [HttpGet("filter-by-natural-language")]
        public async Task<IActionResult> FilterByNaturalLanguage([FromQuery] string query)
        {
            var results = await _stringAnalyzerServ.FilterByNaturalLanguageAsync(query);
            return Ok(results);
        }

        [HttpDelete("{value}")]
        public async Task<IActionResult> Delete(string value)
        {
            var success = await _stringAnalyzerServ.DeleteStringAsync(value);
            return success ? NoContent() : NotFound();
        }
    }
}
