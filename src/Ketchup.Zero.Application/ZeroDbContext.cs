using System.Collections.Concurrent;
using Ketchup.Profession.Domain.Implementation;
using Ketchup.Profession.ORM.EntityFramworkCore.Context;
using Microsoft.EntityFrameworkCore;

namespace Ketchup.Zero.Application
{
    public class ZeroDbContext : DbContext, IEfCoreContext
    {
        private readonly ConcurrentDictionary<string, object> _allSet = new ConcurrentDictionary<string, object>();

        public DbSet<TEntity> CreateSet<TEntity, TPrimaryKey>()
            where TEntity : EntityOfTPrimaryKey<TPrimaryKey>
        {
            var key = typeof(TEntity).FullName;
            object result;

            if (!_allSet.TryGetValue(key, out result))
            {
                result = Set<TEntity>();
                _allSet.TryAdd(key, result);
            }
            return Set<TEntity>();
        }
    }
}
