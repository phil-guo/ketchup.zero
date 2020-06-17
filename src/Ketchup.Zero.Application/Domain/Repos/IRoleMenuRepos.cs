using System;
using System.Collections.Generic;
using System.Text;
using Ketchup.Profession.ORM.EntityFramworkCore.Repository;

namespace Ketchup.Zero.Application.Domain.Repos
{
    public interface IRoleMenuRepos : IEfCoreRepository<SysRoleMenu, int>
    {
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        bool BatchInsert(List<SysRoleMenu> entities);
    }
}
