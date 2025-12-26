using AutoMapper;
using Flownix.Backend.Contracts.DTOs.CreateDTOs;
using Flownix.Backend.Domain.Entities;

namespace Flownix.Backend.Application.Mappings.CreateDTOMappings
{
    public class WaterObjectCreateMapping : Profile
    {
        public WaterObjectCreateMapping()
        {
            CreateMap<CreateWaterObjectDto, WaterObject>();
        }
    }
}