using Applogiq.BlogModule.Domain;
using Applogiq.BlogModule.DTOs.Blogs;
using Applogiq.BlogModule.Services;
using Applogiq.Common.EFCore.Model;
using Applogiq.IdentityServer.Constants;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Applogiq.BlogModule.Controllers
{

    public class BlogController : DefaultBaseController
    {
        private readonly IBlogService blogService;
        private readonly IMemoryCache cache;
        private readonly IMapper mapper;

        public BlogController(IBlogService blogService,
            IMemoryCache cache,
            IMapper mapper)
        {
            this.blogService = blogService;
            this.cache = cache;
            this.mapper = mapper;
        }

        [HttpGet("filter/{pageNo}")]
        public async Task<ActionResult<Paginated<BlogDTO>>> FilterAsync(int pageNo,string? category,string? author)
        {
            var result = await blogService.FilterAsync(pageNo, category, author);
            var blogDtos = mapper.Map<IEnumerable<BlogDTO>>(result.Item);
            return Ok(new Paginated<BlogDTO>(result.TotalCount, blogDtos));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BlogDTO>> GetByIdAsync(int id)
        {
            var cacheKey = "Blog" + id;
            var cachedData = cache.Get<Blog>(cacheKey);

            if (cachedData == null)
            {
                cachedData = await blogService.GetByIdAsync(id);
                cache.Set(cacheKey, cachedData, TimeSpan.FromMinutes(10)); 
            }

            var blogWithCommentDTO = mapper.Map<BlogDTO>(cachedData);

            return Ok(blogWithCommentDTO) ?? (ActionResult<BlogDTO>)NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreateBlogDTO request)
        {
            var blog = mapper.Map<Blog>(request);

            await blogService.CreateAsync(blog);
            
            return Ok(new { id = blog.Id });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, UpdateBlogDTO request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }
            var blog = mapper.Map<Blog>(request);

            await blogService.UpdateAsync(blog);

            var cacheKey = "Blog" + id;
            cache.Remove(cacheKey);

            return NoContent();
        }

        [Authorize(Roles = RoleConstant.SuperAdminRoleName)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var blog = await blogService.GetByIdAsync(id);

            if (blog == null)
            {
                return NotFound();
            }

            await blogService.DeleteAsync(blog);

            var cacheKey = "Blog" + id;
            cache.Remove(cacheKey);

            return NoContent();
        }
    }
}

