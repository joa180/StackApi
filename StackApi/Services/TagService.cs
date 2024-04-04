using StackApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace StackApi.Services
{
    public class TagService
    {
        private readonly TagsDbContext _dbContext;

        public TagService(TagsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<decimal> CalculateTagPercentageAsync(string tagName)
        {
            var totalTagsCount = await _dbContext.Tags.SumAsync(t => t.Count);

            var tag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
            if (tag == null)
                throw new ArgumentException($"Tag '{tagName}' not found.");

            decimal tagPercentage = ((decimal)tag.Count / totalTagsCount) * 100;

            return tagPercentage;
        }
        public async Task SaveTagsAsync(IEnumerable<string> tags)
        {
            foreach (var tagName in tags)
            {
                var existingTag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                if (existingTag != null)
                {
                    // Jeśli tag już istnieje w bazie danych, możemy zaktualizować jego licznik
                    existingTag.Count++;
                }
                else
                {
                    // Jeśli tag nie istnieje, dodajemy go do bazy danych
                    _dbContext.Tags.Add(new Tag { Name = tagName, Count = 1 });
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
