namespace Applogiq.Common.EFCore.Extention
{
    public static class EntityPaginationExtention
    {
        const int PAGE_SIZE = 10;
        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> result, int pageIndex)
        {
            result = result.Skip(pageIndex * PAGE_SIZE);
            result = result.Take(PAGE_SIZE);

            return result;
        }

    }

}
