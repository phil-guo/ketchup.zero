using Ketchup.Zero.Application.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ketchup.Zero.Application.Seed
{
    public class SeedRole : IEntityTypeConfiguration<SysRole>
    {
        public void Configure(EntityTypeBuilder<SysRole> builder)
        {
            builder.HasData(new SysRole()
            {
                Id = 1,
                Name = "管理员"
            });
        }
    }
}
