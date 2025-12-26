using Flownix.Backend.Contracts.DTOs.CreateDTOs;
using Flownix.Backend.Domain.Entities;


namespace Flownix.Backend.Application.Common.Mappings.CreateDTOMappings
{
    public class UserCreateMapping : AutoMapper.Profile
    {
        public UserCreateMapping()
        {
            CreateMap<UserCreateDto, User>();
        }
    }
}