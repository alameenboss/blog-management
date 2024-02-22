using Applogiq.BlogModule.Domain;
using Applogiq.Common.EFCore.Extention;
using Applogiq.Common.EFCore.Model;
using Microsoft.EntityFrameworkCore;

namespace Applogiq.BlogModule.Repositories
{
    public class BlogRepository : IBlogRepository
    {
        protected BlogDbContext dbContext;
        public BlogRepository(BlogDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CreateAsync(Blog entity)
        {
            await dbContext
                .Blogs
                .AddAsync(entity);
        }

        public void Delete(Blog entity)
        {
            dbContext
                .Blogs
                .Remove(entity);
        }

        public async Task<Blog?> GetByIdAsync(int id)
        {
          var blog = await dbContext
                .Blogs
                .AsNoTracking()
                .Where(x => x.Id.Equals(id))
                .FirstOrDefaultAsync();

            return blog;
        }

        public void Update(Blog entity)
        {
            dbContext
                .Blogs
                .Update(entity);
        }

       public async Task<Paginated<Blog>> FilterAsync(int pageNo, string? category, string? author)
        {
            var query = dbContext
                        .Blogs
                        .AsQueryable();

            if (!string.IsNullOrEmpty(category) || !string.IsNullOrEmpty(author))
            {
                query = query.Where(x => x.Author.Contains(category) || x.Author.Contains(author));
            }

            var totalCount = await query.CountAsync();

            
            if(pageNo == 0)
            {
                pageNo = 1;
            }
            query = query.ApplyPagination(pageNo - 1);

            var item = await query.ToListAsync();

            return new Paginated<Blog>(totalCount, item);
        }
    }

}
