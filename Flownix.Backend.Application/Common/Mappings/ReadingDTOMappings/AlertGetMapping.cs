// Flownix.Backend.Application/Mappings/ReadingDTOMappings/AlertGetMapping.cs
using AutoMapper;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using Flownix.Backend.Domain.Entities;

namespace Flownix.Backend.Application.Mappings.ReadingDTOMappings
{
    public class AlertGetMapping : Profile
    {
        public AlertGetMapping()
        {
            CreateMap<Alert, AlertDto>()
                .ForMember(dest => dest.WaterObjectName,
                    opt => opt.MapFrom(src => src.WaterObject != null ? src.WaterObject.Name : string.Empty))
                .ForMember(dest => dest.SensorType,
                    opt => opt.MapFrom(src => src.Sensor != null ? src.Sensor.Type.ToString() : null))
                .ForMember(dest => dest.SensorUnit,
                    opt => opt.MapFrom(src => src.Sensor != null ? src.Sensor.Unit : null));
        }
    }
}