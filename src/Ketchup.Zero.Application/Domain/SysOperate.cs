using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Ketchup.Profession.Domain.Implementation;

namespace Ketchup.Zero.Application.Domain
{
    [Table("sys_operate")]
    public class SysOperate : EntityOfTPrimaryKey<int>
    {
        /// <summary>
        ///     按钮名称
        /// </summary>
        [StringLength(20)]
        [Required]
        public string Name { get; set; }

        /// <summary>
        ///     备注
        /// </summary>
        [StringLength(int.MaxValue)]
        public string Remark { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        [Required]
        public int Unique { get; set; }
    }
}
