using Applogiq.BlogModule.Domain;
using Applogiq.BlogModule.Repositories;
using Applogiq.Common.EFCore;
using Applogiq.Common.EFCore.Model;

namespace Applogiq.BlogModule.Services
{
    public class CommentService : ICommentService
    {

        private readonly ICommentRepository commentRepository;
        private readonly IUnitOfWork unitOfWork;

        public CommentService(ICommentRepository commentRepository, IUnitOfWork unitOfWork)
        {
            this.commentRepository = commentRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Comment>?> GetByBlogIdAsync(int blogId)
        {
            return await commentRepository.GetByBlogIdAsync(blogId);
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await commentRepository.GetByIdAsync(id);
        }

        public async Task CreateAsync(Comment entity)
        {
            await commentRepository.CreateAsync(entity);
            await unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(Comment entity)
        {
            commentRepository.Delete(entity);
            await unitOfWork.CommitAsync();
        }
    }
}
