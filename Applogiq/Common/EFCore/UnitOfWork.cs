using Applogiq.BlogModule;
using Applogiq.Common.EFCore.Model;
using Microsoft.EntityFrameworkCore;

namespace Applogiq.Common.EFCore
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BlogDbContext _dbContext;


        public UnitOfWork(BlogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CommitAsync()
        {
            IEnumerable<IEntity> addedEntities = _dbContext
                                    .ChangeTracker
                                    .Entries()
                                    .Where(c => c.State == EntityState.Added)
                                    .Select(c => c.Entity)
                                    .OfType<IEntity>();

            foreach (IEntity entity in addedEntities)
            {
                entity.CreateDate = DateTime.UtcNow;
                entity.CreatedBy = "";
                entity.ModifiedDate = DateTime.UtcNow;
                entity.ModifiedBy = "";
            }
            IEnumerable<IEntity> updatedEntities = _dbContext
                                   .ChangeTracker
                                   .Entries()
                                   .Where(c => c.State == EntityState.Modified)
            .Select(c => c.Entity)
                                   .OfType<IEntity>();

            foreach (IEntity entity in updatedEntities)
            {
                entity.ModifiedDate = DateTime.UtcNow;
                entity.ModifiedBy = "";
            }



            using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

        }
    }
}