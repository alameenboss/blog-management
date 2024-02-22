using Applogiq.BlogModule.Domain;
using Applogiq.BlogModule.DTOs.Comments;
using Applogiq.BlogModule.Services;
using Applogiq.IdentityServer.Constants;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Applogiq.BlogModule.Controllers
{

    public class CommentController : DefaultBaseController
    {
        private readonly ICommentService commentService;
        private readonly IMapper mapper;

        public CommentController(ICommentService commentService, IMapper mapper)
        {
            this.commentService = commentService;
            this.mapper = mapper;
        }

        [Authorize]
        [HttpGet("blog/{id}")]
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetByBlogIdAsync(int id)
        {
            var result = await commentService.GetByBlogIdAsync(id);
            var commentDtos = mapper.Map<IEnumerable<CommentDTO>>(result);
            return Ok(commentDtos) ?? (ActionResult<IEnumerable<CommentDTO>>)NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreateCommentDTO request)
        {
            var commentDto = mapper.Map<Comment>(request);

            await commentService.CreateAsync(commentDto);
            
            return Ok(new { id = commentDto.Id });
        }

        

        [Authorize(Roles = RoleConstant.SuperAdminRoleName)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var comment = await commentService.GetByIdAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            await commentService.DeleteAsync(comment);

            return NoContent();
        }
    }
}

