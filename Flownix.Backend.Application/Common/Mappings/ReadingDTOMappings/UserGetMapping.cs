using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using Flownix.Backend.Domain.Entities;

namespace Flownix.Backend.Application.Common.Mappings.ReadingDTOMappings
{
    public class UserGetMapping : AutoMapper.Profile
    {
        public UserGetMapping()
        {
            CreateMap<User, UserDto>();
        }
    }
}