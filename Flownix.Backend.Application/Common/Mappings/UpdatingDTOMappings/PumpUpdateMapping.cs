using AutoMapper;
using Flownix.Backend.Contracts.DTOs.UpdateDTOs;
using Flownix.Backend.Domain.Entities;

namespace Flownix.Backend.Application.Mappings.UpdatingDTOMappings
{
    public class PumpUpdateMapping : Profile
    {
        public PumpUpdateMapping()
        {
            CreateMap<UpdatePumpDto, Pump>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.WaterObjectId, opt => opt.Ignore()) 
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null));
        }
    }
}
