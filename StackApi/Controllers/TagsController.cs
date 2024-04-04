using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackApi.Entities;
using StackApi.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace StackApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly StackOverflowAPIClient _stackOverflowApiClient;
        private readonly TagService _tagService;
        private readonly TagsDbContext _tagsDbContext;
        private readonly ILogger<TagsController> _logger;

        //wstrzykiwanie zależności przez konstruktor
        public TagsController(StackOverflowAPIClient stackOverflowApiClient, TagService tagService, TagsDbContext tagsDbContext, ILogger<TagsController> logger)
        {
            //zależności
            _stackOverflowApiClient = stackOverflowApiClient;
            _tagService = tagService;
            _tagsDbContext = tagsDbContext;
            _logger = logger;
        }
        //obsługa pobierania wszystkich tagów
        [HttpGet("get-tags")]
        public async Task<ActionResult<string[]>> GetTags(int page = 1, int pageSize = 100)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve tags...");

                var tags = await _stackOverflowApiClient.GetTags(page, pageSize);

                _logger.LogInformation("Tags retrieved successfully.");

                await _tagService.SaveTagsAsync(tags);

                return Ok(tags);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching tags.");
                return StatusCode(500, $"An error occurred while fetching tags from StackOverflow API: {ex.Message}");
            }
        }
        //obsługa procentowego udziału
        [HttpGet("tag-percentage")]
        public async Task<IActionResult> GetTagPercentage(string tagName)
        {
            try
            {
                // Wywołanie metody z serwisu, aby obliczyć procentowy udział tagu
                var tagPercentage = await _tagService.CalculateTagPercentageAsync(tagName);

                // Zwróć procentowy udział tagu w ciele odpowiedzi
                return Ok($"Procentowy udział tagu '{tagName}': {tagPercentage}%");
            }
            catch (ArgumentException ex)
            {
                // Obsługa błędów, np. gdy tag nie istnieje w bazie danych
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Obsługa innych błędów
                return StatusCode(500, $"Wystąpił błąd: {ex.Message}");
            }
        }
        //obsługa stronicowania
        [HttpGet("paged-tags")]
        public async Task<IActionResult> GetPagedTags(int pageNumber = 1, int pageSize = 10, string sortOrder = "name")
        {
            try
            {
                // Pobierz tagi z bazy danych
                var tagsQuery = _tagsDbContext.Tags.AsQueryable();

                // Sortowanie
                tagsQuery = sortOrder switch
                {
                    "name_desc" => tagsQuery.OrderByDescending(t => t.Name),
                    "count" => tagsQuery.OrderBy(t => t.Count),
                    "count_desc" => tagsQuery.OrderByDescending(t => t.Count),
                    _ => tagsQuery.OrderBy(t => t.Name) // Sortowanie domyślne
                };

                // Stronicowanie
                var paginatedTags = await PaginatedList<Tag>.CreateAsync(tagsQuery, pageNumber, pageSize);

                return Ok(paginatedTags);
            }
            catch (Exception ex)
            {
                // Obsługa błędów
                return StatusCode(500, $"Failed to fetch paged tags: {ex.Message}");
            }
        }
        //obsługa ponownego pobrania
        [HttpPost("refresh-tags")]
        public async Task<IActionResult> RefreshTags()
        {
            try
            {
                // Wywołaj metodę z klasy klienta do ponownego pobrania tagów z API StackOverflow
                var tags = await _stackOverflowApiClient.RefreshTags();

                // Zapisz ponownie pobrane tagi do bazy danych
                await _tagService.SaveTagsAsync(tags);

                return Ok("Tags refreshed and saved to database successfully");
            }
            catch (Exception ex)
            {
                // Obsługa błędów
                return StatusCode(500, $"Failed to refresh tags: {ex.Message}");
            }
        }
    }
}
