using Applogiq.Common.EFCore.Model;

namespace Applogiq.BlogModule.Domain
{
    public class Comment : Entity
    {

        public Comment()
        {

        }

        public string Content { get; set; }
        public string Author { get; set; }
        public int BlogId { get; set; } 
        public virtual Blog Blog { get; set; }   
    }
}
