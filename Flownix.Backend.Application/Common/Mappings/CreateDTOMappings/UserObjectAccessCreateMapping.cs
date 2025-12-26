using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Flownix.Backend.Contracts.DTOs.CreateDTOs;
using Flownix.Backend.Domain.Entities;

namespace Flownix.Backend.Application.Mappings.CreateDTOMappings
{
    public class UserObjectAccessCreateMapping : Profile
    {
        public UserObjectAccessCreateMapping()
        {
            CreateMap<UserObjectAccessCreateDto, UserObjectAccess>();
        }
    }
}