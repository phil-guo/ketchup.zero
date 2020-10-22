using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Ketchup.Core.Attributes;
using Ketchup.Core.Kong.Attribute;
using Ketchup.Permission;
using Ketchup.Profession.AutoMapper;
using Ketchup.Profession.ORM.EntityFramworkCore.Repository;
using Ketchup.Zero.Application.Domain;
using Ketchup.Zero.Application.Domain.Repos;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ketchup.Zero.Application.Services.Operate
{
    [Service(Name = nameof(RpcOperate), Package = "Ketchup.Permission",
        TypeClientName = nameof(RpcOperate.RpcOperateClient))]
    public class OperateService : RpcOperate.RpcOperateBase
    {
        private readonly IEfCoreRepository<SysOperate, int> _operate;
        private readonly IRoleMenuRepos _roleMenu;
        private readonly IMapper _mapper;

        public OperateService(IEfCoreRepository<SysOperate, int> operate, IRoleMenuRepos roleMenu, IMapper mapper)
        {
            _operate = operate;
            _roleMenu = roleMenu;
            _mapper = mapper;
        }

        /// <summary>
        ///     分页查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [KongRoute(Name = "operates.PageSerachOperate", Tags = new[] { "operate" },
            Methods = new[] { "POST", "OPTIONS" }, Paths = new[] { "/zero/operates/PageSerachOperate" })]
        [ServiceRoute(Name = "operates", MethodName = nameof(PageSerachOperate))]
        public override Task<OperatesResponse> PageSerachOperate(SearchOperate request, ServerCallContext context)
        {
            var query = _operate.GetAll().AsNoTracking();

            if (SearchFilter(request) != null)
                query = query.Where(SearchFilter(request));

            query = OrderFilter() != null
                ? query.OrderByDescending(OrderFilter())
                : query.OrderByDescending(item => item.Id);

            var total = query.Count();

            var result = query.Skip(request.PageMax * (request.PageIndex - 1))
                .Take(request.PageMax)
                .ToList();

            var date = new OperatesResponse { Total = total };

            ConvertToEntities(result).ForEach(item => { date.Datas.Add(item); });

            return Task.FromResult(date);
        }

        /// <summary>
        ///     创建或修改
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [KongRoute(Name = "operates.CreateOrEditOperate", Tags = new[] { "operate" },
            Methods = new[] { "POST", "OPTIONS" }, Paths = new[] { "/zero/operates/CreateOrEditOperate" })]
        [ServiceRoute(Name = "operates", MethodName = nameof(CreateOrEditOperate))]
        public override Task<OperateDto> CreateOrEditOperate(OperateDto request, ServerCallContext context)
        {
            SysOperate data = null;
            if (request.Id == 0)
            {
                var entity = _operate.GetAll().OrderBy(item => item.Id).LastOrDefault();
                if (entity != null)
                    request.Unique = entity.Unique + 1;
                else
                    request.Unique = 10001;

                data = _operate.Insert(_mapper.Map<SysOperate>(request));
            }
            else
            {
                data = _operate.SingleOrDefault(item => item.Id == request.Id);
                data.Name = request.Name;
                data.Remark = request.Remark;
                data = _operate.Update(data);
            }

            return Task.FromResult(_mapper.Map<OperateDto>(data));
        }

        /// <summary>
        ///     获取菜单的功能
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [KongRoute(Name = "operates.GetMenuOfOperate", Tags = new[] { "operate" },
            Methods = new[] { "POST", "OPTIONS" }, Paths = new[] { "/zero/operates/GetMenuOfOperate" })]
        [ServiceRoute(Name = "operates", MethodName = nameof(GetMenuOfOperate))]
        public override Task<MenuOfOperateReponse> GetMenuOfOperate(MenuOfOperateRequest request,
            ServerCallContext context)
        {
            var roleMenu =
                _roleMenu.SingleOrDefault(item => item.RoleId == request.RoleId && item.MenuId == request.MenuId);
            var idNos = new MenuOfOperateReponse();
            JsonConvert.DeserializeObject<List<int>>(roleMenu?.Operates).ForEach(id =>
            {
                var operate = _operate.SingleOrDefault(item => item.Id == id);
                idNos.Datas.Add(operate?.Unique.ToString());
            });
            return Task.FromResult(idNos);
        }

        [KongRoute(Name = "operates.RemoveOperate", Tags = new[] { "operate" },
            Methods = new[] { "POST", "OPTIONS" }, Paths = new[] { "/zero/operates/RemoveOperate" })]
        [ServiceRoute(Name = "operates", MethodName = nameof(RemoveOperate))]
        public override Task<RemoveResponse> RemoveOperate(RemoveRequest request, ServerCallContext context)
        {
            var response = new RemoveResponse();
            try
            {
                _operate.Delete(request.Id);
                response.IsComplete = true;
                return Task.FromResult(response);
            }
            catch
            {
                response.IsComplete = false;
                return Task.FromResult(response);
            }
        }

        protected Expression<Func<SysOperate, bool>> SearchFilter(SearchOperate search)
        {
            Expression<Func<SysOperate, bool>> expression = item => true;

            if (!string.IsNullOrEmpty(search.Name))
                expression = expression.And(item => item.Name.Contains(search.Name));

            return expression;
        }

        protected Expression<Func<SysOperate, int>> OrderFilter()
        {
            return null;
        }

        protected List<OperateDto> ConvertToEntities(List<SysOperate> entities)
        {
            return _mapper.Map<List<OperateDto>>(entities);
        }
    }
}