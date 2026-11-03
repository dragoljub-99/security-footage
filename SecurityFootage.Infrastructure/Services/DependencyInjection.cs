using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecurityFootage.Domain.Interfaces;
using SecurityFootage.Infrastructure.Abstractions;
using SecurityFootage.Infrastructure.Exceptions;
using SecurityFootage.Infrastructure.Persistence;
using SecurityFootage.Infrastructure.Services;
using SecurityFootageExercise.Infrastructure.Services;

namespace SecurityFootage.Infrastructure.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
                                                           IConfiguration conf)
        {
            
            var connString = conf["AZURE_CONNSTRING"];

            if (string.IsNullOrEmpty(connString))
            {
                throw new ConnectionStringNullException("Azure conn string missing");
            }

             services.AddDbContext<SecurityFootageDbContext>(options => 
                                            options.UseSqlServer(connString));

            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
            services.AddScoped<IFootageAccessService,FootageAccessService>();
            services.AddScoped<IEventQueueService, EventQueueService>();

            return services;
        }
    }
}