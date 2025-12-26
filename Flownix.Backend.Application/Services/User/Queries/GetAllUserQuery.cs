using AutoMapper;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using Flownix.Backend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

namespace Flownix.Backend.Application.Services.User.Queries
{
    public class GetAllUserQuery : IRequest<List<UserDto>>
    {
        public GetAllUserQuery() { }
    }

    public class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, List<UserDto>>
    {
        private readonly IFlownixDbContext _context;

        public GetAllUserQueryHandler(IFlownixDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDto>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users.Select(u => new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = (Contracts.DTOs.Enums.Role)u.Role, 
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                LastLoginAt = u.LastLoginAt,
            }).ToListAsync();
        }
    }
}