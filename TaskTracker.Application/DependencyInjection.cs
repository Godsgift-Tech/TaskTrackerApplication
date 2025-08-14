using FluentValidation;
using MediatR.CommandQuery;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Features.Tasks.Command.CompleteTaskCommand;

namespace TaskTracker.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // Automatically register all FluentValidation validators in this assembly
            services.AddValidatorsFromAssembly(typeof(CompleteTaskCommandValidator).Assembly);

            return services;
        }
    }
}
