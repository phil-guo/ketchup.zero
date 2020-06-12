using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Ketchup.Profession.Domain.Implementation;

namespace Ketchup.Zero.Application.Domain
{
    [Table("sys_user")]
    public class SysUser : EntityOfTPrimaryKey<int>
    {
        /// <summary>
        ///     角色id
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        ///     用户名
        /// </summary>
        [StringLength(32)]
        [Required]
        public string UserName { get; set; }

        /// <summary>
        ///     密码
        /// </summary>
        [StringLength(500)]
        [Required]
        public string Password { get; set; }
    }
}
