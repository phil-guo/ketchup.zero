using System;
using System.Collections.Generic;
using System.Text;
using Ketchup.Profession.ORM.EntityFramworkCore.UntiOfWork.Implementation;

namespace Ketchup.Zero.Application
{
    public class ZeroUnitOfWork : EfUnitOfWork<ZeroDbContext>
    {
        public ZeroUnitOfWork(ZeroDbContext defaultDbContext) : base(defaultDbContext)
        {
        }
    }
}
