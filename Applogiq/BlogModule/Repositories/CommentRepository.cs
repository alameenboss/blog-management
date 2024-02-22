using Applogiq.BlogModule.Domain;
using Applogiq.Common.EFCore.Extention;
using Applogiq.Common.EFCore.Model;
using Microsoft.EntityFrameworkCore;

namespace Applogiq.BlogModule.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        protected BlogDbContext dbContext;
        public CommentRepository(BlogDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CreateAsync(Comment entity)
        {
            await dbContext
                .Comments
                .AddAsync(entity);
        }

        public void Delete(Comment entity)
        {
            dbContext
                .Comments
                .Remove(entity);
        }

        public async Task<IEnumerable<Comment>> GetByBlogIdAsync(int blogId)
        {
           var blogs = await dbContext
               .Comments
               .AsNoTracking()
               .Where(x => x.BlogId.Equals(blogId))
               .ToListAsync();

            return blogs;
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
          var blog = await dbContext
                .Comments
                .AsNoTracking()
                .Where(x => x.Id.Equals(id))
                .FirstOrDefaultAsync();
            return blog;
        }


    }

}
