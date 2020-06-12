using System;
using System.Collections.Generic;
using System.Text;
using Ketchup.Zero.Application.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ketchup.Zero.Application.Seed
{
    public class SeedMenu : IEntityTypeConfiguration<SysMenu>
    {
        public void Configure(EntityTypeBuilder<SysMenu> builder)
        {
            throw new NotImplementedException();
        }
    }
}
