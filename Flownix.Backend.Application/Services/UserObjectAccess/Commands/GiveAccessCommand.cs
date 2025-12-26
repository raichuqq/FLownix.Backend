using AutoMapper;
using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.CreateDTOs;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using Flownix.Backend.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flownix.Backend.Application.Services.UserObjectAccess.Commands
{
    public class GiveAccessCommand : IRequest<UserObjectAccessDto>
    {
        public required UserObjectAccessCreateDto Access { get; set; }
    }

    public class GiveAccessCommandHandler : IRequestHandler<GiveAccessCommand, UserObjectAccessDto>
    {
        private readonly IFlownixDbContext _context;
        private readonly IMapper _mapper;

        public GiveAccessCommandHandler(IFlownixDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserObjectAccessDto> Handle(
            GiveAccessCommand request,
            CancellationToken cancellationToken)
        {
            // Проверяем, существует ли уже доступ
            var existingAccess = await _context.UserObjectAccesses
                .FirstOrDefaultAsync(ua =>
                    ua.UserId == request.Access.UserId &&
                    ua.WaterObjectId == request.Access.WaterObjectId,
                    cancellationToken);

            if (existingAccess != null)
            {
                return _mapper.Map<UserObjectAccessDto>(existingAccess);
            }

            // Проверяем, существуют ли пользователь и объект
            var userExists = await _context.Users
                .AnyAsync(u => u.Id == request.Access.UserId, cancellationToken);

            var waterObjectExists = await _context.WaterObjects
                .AnyAsync(w => w.Id == request.Access.WaterObjectId, cancellationToken);

            if (!userExists || !waterObjectExists)
            {
                throw new Exception("User or WaterObject not found");
            }

            // Создаём доступ
            var access = _mapper.Map<Domain.Entities.UserObjectAccess>(request.Access);
            access.CreatedAt = DateTime.UtcNow;

            _context.UserObjectAccesses.Add(access);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<UserObjectAccessDto>(access);
        }
    }
}