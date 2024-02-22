using Applogiq.BlogModule.Domain;
using Applogiq.Common.EFCore.Model;

namespace Applogiq.BlogModule.Repositories
{
    public interface IBlogRepository
    {
        Task<Paginated<Blog>> FilterAsync(int pageNo, string? category, string? author);

        Task<Blog?> GetByIdAsync(int id);

        Task CreateAsync(Blog entity);

        void Update(Blog entity);

        void Delete(Blog entity);
    }
}
