using System.Collections.Generic;
using Ketchup.Profession.ORM.EntityFramworkCore.Repository.Implementation;
using Ketchup.Profession.ORM.EntityFramworkCore.UntiOfWork;

namespace Ketchup.Zero.Application.Domain.Repos.Imp
{
    public class RoleMenuRepos : EfCoreRepository<SysRoleMenu, int>, IRoleMenuRepos
    {
        private readonly IEfUnitOfWork _unitOfWork;
        public RoleMenuRepos(IEfUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool BatchInsert(List<SysRoleMenu> entities)
        {
            entities.ForEach(item => { GetSet().Add(item); });
            return _unitOfWork.Commit() != 0;
        }
    }
}
