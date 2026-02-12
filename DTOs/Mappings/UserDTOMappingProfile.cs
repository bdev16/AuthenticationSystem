using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationSystem.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationSystem.DTOs.Mappings
{
    public class UserDTOMappingProfile : Profile
    {
        public UserDTOMappingProfile()
        {
            CreateMap<IdentityUser, UserDTO>().ReverseMap();
            CreateMap<UserDTO, IdentityUser>().ReverseMap();
        }
    }
}