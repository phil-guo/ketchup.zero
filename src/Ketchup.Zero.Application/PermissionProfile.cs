using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Ketchup.Permission;
using Ketchup.Zero.Application.Domain;

namespace Ketchup.Zero.Application
{
    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {
            CreateMap<MenuDto, SysMenu>();
        }
    }
}
