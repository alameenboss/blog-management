using Applogiq.BlogModule.Domain;
using Applogiq.BlogModule.DTOs.Blogs;
using Applogiq.BlogModule.DTOs.Comments;
using AutoMapper;

namespace Applogiq.BlogModule.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Blog, BlogDTO>();
            CreateMap<CreateBlogDTO, Blog>();
            CreateMap<UpdateBlogDTO, Blog>();

            CreateMap<Comment, CommentDTO>().ReverseMap();
            CreateMap<CreateCommentDTO, Comment>();

        }
    }
}