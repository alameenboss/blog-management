using Applogiq.Common.EFCore.Model;

namespace Applogiq.BlogModule.Domain
{
    public class Blog : Entity
    {
        public Blog()
        {
            Comments = new List<Comment>();
        }

        public string Title { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public DateTime PublishDate { get; set; }
        public string Author { get; set; }
        public virtual List<Comment> Comments { get; set; }
    }
}
