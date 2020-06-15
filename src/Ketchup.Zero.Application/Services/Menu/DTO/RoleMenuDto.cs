using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Zero.Application.Services.Menu.DTO
{
    public class RoleMenuDto
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }

        public string Path { get; set; }

        public List<RoleMenuDto> Children { get; set; } = new List<RoleMenuDto>();

        public string Operates { get; set; }
    }
}
