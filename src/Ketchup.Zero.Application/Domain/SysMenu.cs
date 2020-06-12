using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Ketchup.Profession.Domain.Implementation;

namespace Ketchup.Zero.Application.Domain
{
    [Table("sys_menu")]
    public class SysMenu : EntityOfTPrimaryKey<int>
    {
        /// <summary>
        ///     父级
        /// </summary>
        public int ParentId { get; set; } = 0;

        /// <summary>
        ///     菜单名称
        /// </summary>
        [StringLength(20)]
        public string Name { get; set; }

        /// <summary>
        ///     菜单地址
        /// </summary>
        [StringLength(20)]
        [Required]
        public string Url { get; set; }

        /// <summary>
        ///     层级
        /// </summary>
        [Column(TypeName = "tinyint(4)")]
        public int Level { get; set; } = 1;

        /// <summary>
        ///     菜单权限(list<int /> json)
        /// </summary>
        [StringLength(100)]
        public string Operates { get; set; }

        /// <summary>
        ///     排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        ///     添加菜单时，操作排序自动加1
        /// </summary>
        public int AddOperateSort()
        {
            Sort += 1;
            return Sort;
        }
    }
}
