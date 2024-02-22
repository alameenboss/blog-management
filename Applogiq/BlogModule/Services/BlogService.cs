using Applogiq.BlogModule.Domain;
using Applogiq.BlogModule.Repositories;
using Applogiq.Common.EFCore;
using Applogiq.Common.EFCore.Model;

namespace Applogiq.BlogModule.Services
{
    public class BlogService : IBlogService
    {

        private readonly IBlogRepository blogRepository;
        private readonly IUnitOfWork unitOfWork;

        public BlogService(IBlogRepository blogRepository, IUnitOfWork unitOfWork)
        {
            this.blogRepository = blogRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<Paginated<Blog>> FilterAsync(int pageNo, string? category, string? author)
        {
            return await blogRepository.FilterAsync(pageNo, category, author);
        }

        public async Task<Blog?> GetByIdAsync(int id)
        {
            return await blogRepository.GetByIdAsync(id);
        }

        public async Task CreateAsync(Blog entity)
        {
            await blogRepository.CreateAsync(entity);
            await unitOfWork.CommitAsync();
        }

        public async Task UpdateAsync(Blog entity)
        {
            var existingModel = await GetByIdAsync(entity.Id);

            if (existingModel != null)
            {
                existingModel.Title = entity.Title;
                existingModel.Content = entity.Content;
                existingModel.Category = entity.Category;
                existingModel.PublishDate = entity.PublishDate;
                existingModel.Author = entity.Author;
                blogRepository.Update(existingModel);
                await unitOfWork.CommitAsync();
            }
        }

        public async Task DeleteAsync(Blog entity)
        {
            blogRepository.Delete(entity);
            await unitOfWork.CommitAsync();
        }
    }
}
