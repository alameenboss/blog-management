namespace Applogiq.Common.EFCore.Model
{
    public class Paginated<T>
    {
        public Paginated()
        {

        }
        public Paginated(int totalCount, IEnumerable<T> item)
        {
            Item = item;
            TotalCount = totalCount;
        }

        public IEnumerable<T> Item { get; set; }
        public int TotalCount { get; set; }

    }

}
