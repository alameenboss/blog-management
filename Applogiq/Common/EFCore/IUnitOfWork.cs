namespace Applogiq.Common.EFCore
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }
}