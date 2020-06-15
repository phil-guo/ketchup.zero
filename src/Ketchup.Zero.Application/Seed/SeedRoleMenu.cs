using System;
using System.Collections.Generic;
using System.Text;
using Ketchup.Zero.Application.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Ketchup.Zero.Application.Seed
{
    public class SeedRoleMenu : IEntityTypeConfiguration<SysRoleMenu>
    {
        public void Configure(EntityTypeBuilder<SysRoleMenu> builder)
        {
            builder.HasData(new SysRoleMenu
            {
                Id = 1,
                MenuId = 1,
                RoleId = 1,
                Operates = JsonConvert.SerializeObject(new List<int>() { })
            }, new SysRoleMenu
            {
                Id = 2,
                MenuId = 2,
                RoleId = 1,
                Operates = JsonConvert.SerializeObject(new List<int>() { 1, 2, 3, 4 })
            }, new SysRoleMenu
            {
                Id = 3,
                MenuId = 3,
                RoleId = 1,
                Operates = JsonConvert.SerializeObject(new List<int>() { 1, 2, 3 })
            }, new SysRoleMenu
            {
                Id = 4,
                MenuId = 4,
                RoleId = 1,
                Operates = JsonConvert.SerializeObject(new List<int>() { 1, 2, 3, 4 })
            });
        }
    }
}
