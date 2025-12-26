using AutoMapper;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using Flownix.Backend.Domain.Entities;

namespace Flownix.Backend.Application.Mappings.ReadingDTOMappings
{
    public class PumpGetMapping : Profile
    {
        public PumpGetMapping()
        {
            CreateMap<Pump, PumpDto>();
        }
    }
}
