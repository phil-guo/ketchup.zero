using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Ketchup.Profession.Domain.Implementation;

namespace Ketchup.Zero.Application.Domain
{
    [Table("sys_roleMenu")]
    public class SysRoleMenu : EntityOfTPrimaryKey<int>
    {
        /// <summary>
        ///     角色id
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        ///     菜单id
        /// </summary>
        public int MenuId { get; set; }

        /// <summary>
        ///     操作权限，(list<object/> json)
        /// </summary>
        [StringLength(200)]
        public string Operates { get; set; }
    }
}
