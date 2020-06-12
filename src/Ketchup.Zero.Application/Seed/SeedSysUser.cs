using System;
using System.Collections.Generic;
using System.Text;
using Ketchup.Profession.Utilis;
using Ketchup.Zero.Application.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ketchup.Zero.Application.Seed
{
    public class SeedSysUser : IEntityTypeConfiguration<SysUser>
    {
        public void Configure(EntityTypeBuilder<SysUser> builder)
        {
            builder.HasData(new SysUser
            {
                Id = 1,
                UserName = "admin",
                Password = "123qwe".Get32MD5One(),
                RoleId = 1
            });
        }
    }
}
