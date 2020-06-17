using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Zero.Application.Services.Role.DTO
{
    public class RolePermissionDto
    {
        public int MenuId { get; set; }

        public List<int> Operates { get; set; } = new List<int>();
    }
}
