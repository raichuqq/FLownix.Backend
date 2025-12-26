using AutoMapper;
using Flownix.Backend.Contracts.DTOs.UpdateDTOs;
using Flownix.Backend.Domain.Entities;

namespace Flownix.Backend.Application.Mappings.UpdatingDTOMappings
{
    public class WaterObjectUpdateMapping : Profile
    {
        public WaterObjectUpdateMapping()
        {
            CreateMap<UpdateWaterObjectDto, WaterObject>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null)); 
        }
    }
}