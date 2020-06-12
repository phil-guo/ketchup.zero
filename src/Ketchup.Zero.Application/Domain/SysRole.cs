using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Ketchup.Profession.Domain.Implementation;

namespace Ketchup.Zero.Application.Domain
{
    [Table("sys_role")]
    public class SysRole : EntityOfTPrimaryKey<int>
    {
        /// <summary>
        ///     角色名称
        /// </summary>
        [StringLength(20)]
        [Required]
        public string Name { get; set; }

        /// <summary>
        ///     备注
        /// </summary>
        [StringLength(int.MaxValue)]
        public string Remark { get; set; }
    }
}
