using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using Flownix.Backend.Domain.Entities;

namespace Flownix.Backend.Application.Mappings.ReadingDTOMappings
{
    public class UserObjectAccessGetMapping : Profile
    {
        public UserObjectAccessGetMapping()
        {
            CreateMap<UserObjectAccess, UserObjectAccessDto>();
        }
    }
}