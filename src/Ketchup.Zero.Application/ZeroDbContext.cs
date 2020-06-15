using System.Collections.Concurrent;
using Ketchup.Profession.Domain.Implementation;
using Ketchup.Profession.ORM.EntityFramworkCore.Context;
using Ketchup.Zero.Application.Config;
using Ketchup.Zero.Application.Domain;
using Ketchup.Zero.Application.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ketchup.Zero.Application
{
    public class ZeroDbContext : DbContext, IEfCoreContext
    {
        public ZeroDbContext(DbContextOptions options) : base(options)
        {
        }

        private readonly ConcurrentDictionary<string, object> _allSet = new ConcurrentDictionary<string, object>();
        public DbSet<SysMenu> SysMenus { get; set; }
        public DbSet<SysOperate> SysOperates { get; set; }
        public DbSet<SysRole> SysRoles { get; set; }
        public DbSet<SysRoleMenu> SysRoleMenus { get; set; }
        public DbSet<SysUser> SysUsers { get; set; }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SeedOperate());
            modelBuilder.ApplyConfiguration(new SeedRole());
            modelBuilder.ApplyConfiguration(new SeedMenu());
            modelBuilder.ApplyConfiguration(new SeedSysUser());
            modelBuilder.ApplyConfiguration(new SeedRoleMenu());
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);
        //    var appConfig = new AppConfig();
        //    optionsBuilder.UseMySql(appConfig.Zero.Connection);
        //}
    }

    public class BabyContextFactory : IDesignTimeDbContextFactory<ZeroDbContext>
    {
        public ZeroDbContext CreateDbContext(string[] args)
        {
            var appConfig = new AppConfig();
            var optionsBuilder = new DbContextOptionsBuilder<ZeroDbContext>();
            optionsBuilder.UseMySql(appConfig.Zero.Connection);
            return new ZeroDbContext(optionsBuilder.Options);
        }
    }
}
