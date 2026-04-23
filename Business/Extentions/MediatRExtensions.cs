using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Business.Handlers.Authentication.Commands;

namespace Business.Extentions
{
    public static class MediatRExtensions
    {
        public static IServiceCollection AddCustomMediatR(this IServiceCollection services)
        {
            // Register handlers from Business layer
            services.AddMediatR(typeof(SendOtpCommand).Assembly);

            return services;
        }
    }
}
