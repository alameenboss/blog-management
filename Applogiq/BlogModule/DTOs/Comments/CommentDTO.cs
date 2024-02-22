namespace Applogiq.BlogModule.DTOs.Comments
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public int BlogId { get; set; }
    }
}
