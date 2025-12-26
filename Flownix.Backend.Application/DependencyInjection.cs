using Flownix.Backend.Application.Common.Behavior;
using Flownix.Backend.Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Flownix.Backend.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());

            return services;
        }

    }
}