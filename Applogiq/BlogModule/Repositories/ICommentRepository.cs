using Applogiq.BlogModule.Domain;

namespace Applogiq.BlogModule.Repositories
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetByBlogIdAsync(int blogId);

        Task<Comment?> GetByIdAsync(int id);

        Task CreateAsync(Comment entity);

        void Delete(Comment entity);
    }
}
