using Applogiq.BlogModule.Domain;

namespace Applogiq.BlogModule.Services
{
    public interface ICommentService
    {
        Task CreateAsync(Comment entity);
        Task DeleteAsync(Comment entity);
        Task<IEnumerable<Comment>> GetByBlogIdAsync(int blogId);
        Task<Comment?> GetByIdAsync(int id);
    }
}
