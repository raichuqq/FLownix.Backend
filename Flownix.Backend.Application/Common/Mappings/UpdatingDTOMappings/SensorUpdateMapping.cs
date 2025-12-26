using AutoMapper;
using Flownix.Backend.Contracts.DTOs.UpdateDTOs;
using Flownix.Backend.Domain.Entities;

namespace Flownix.Backend.Application.Mappings.UpdatingDTOMappings
{
    public class SensorUpdateMapping : Profile
    {
        public SensorUpdateMapping()
        {
            CreateMap<UpdateSensorDto, Sensor>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null));
        }
    }
}