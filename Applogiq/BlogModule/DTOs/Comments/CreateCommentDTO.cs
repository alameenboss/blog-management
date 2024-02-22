namespace Applogiq.BlogModule.DTOs.Comments
{
    public class CreateCommentDTO
    {
        public string Content { get; set; }
        public string Author { get; set; }
        public int BlogId { get; set; }
    }
}
