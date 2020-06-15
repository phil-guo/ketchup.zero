using AutoMapper;
using Ketchup.Permission;
using Ketchup.Zero.Application.Domain;

namespace Ketchup.Zero.Application
{
    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {
            CreateMap<SysMenu, MenuDto>()
                .ForMember(destination => destination.Icon, opt => opt.NullSubstitute(""));
        }
    }
}