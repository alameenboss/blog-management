namespace Applogiq.BlogModule.DTOs.Blogs
{
    public class CreateBlogDTO
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public DateTime PublishDate { get; set; }
        public string Author { get; set; }
    }
}
