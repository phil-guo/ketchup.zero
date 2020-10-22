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
            CreateMap<MenuDto, SysMenu>()
                .ForMember(destination => destination.Icon, opt => opt.NullSubstitute(""));


            CreateMap<CreateOrEditMenuRequest, SysMenu>()
                .ForMember(destination => destination.Icon, opt => opt.NullSubstitute(""))
                .ForMember(destination => destination.Sort, opt => opt.Ignore());
            CreateMap<SysMenu, CreateOrEditMenuRequest>()
                .ForMember(destination => destination.Icon, opt => opt.NullSubstitute(""));



            CreateMap<SysRole, RoleDto>()
                .ForMember(destination => destination.Name, opt => opt.NullSubstitute(""))
                .ForMember(destination => destination.Remark, opt => opt.NullSubstitute(""));
            CreateMap<RoleDto, SysRole>()
                .ForMember(destination => destination.Name, opt => opt.NullSubstitute(""))
                .ForMember(destination => destination.Remark, opt => opt.NullSubstitute(""));


            CreateMap<SysOperate, OperateDto>()
                .ForMember(destination => destination.Name, opt => opt.NullSubstitute(""))
                .ForMember(destination => destination.Remark, opt => opt.NullSubstitute(""));
            CreateMap<OperateDto, SysOperate>()
                .ForMember(destination => destination.Name, opt => opt.NullSubstitute(""))
                .ForMember(destination => destination.Remark, opt => opt.NullSubstitute(""));



            CreateMap<SysUser, SysUserDto>()
                .ForMember(destination => destination.Password, opt => opt.NullSubstitute(""))
                .ForMember(destination => destination.UserName, opt => opt.NullSubstitute(""));

            CreateMap<SysUserDto, SysUser>()
                .ForMember(destination => destination.Password, opt => opt.NullSubstitute(""))
                .ForMember(destination => destination.UserName, opt => opt.NullSubstitute(""));

        }
    }
}