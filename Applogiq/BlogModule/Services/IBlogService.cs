using Applogiq.BlogModule.Domain;
using Applogiq.Common.EFCore.Model;

namespace Applogiq.BlogModule.Services
{
    public interface IBlogService
    {
        Task CreateAsync(Blog entity);
        Task DeleteAsync(Blog entity);
        Task<Paginated<Blog>> FilterAsync(int pageNo, string? category, string? author);
        Task<Blog?> GetByIdAsync(int id);
        Task UpdateAsync(Blog entity);
    }
}
