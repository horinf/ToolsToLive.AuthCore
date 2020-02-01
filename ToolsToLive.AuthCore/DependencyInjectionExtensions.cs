using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToolsToLive.AuthCore.Interfaces;
using ToolsToLive.AuthCore.Interfaces.Model;
using ToolsToLive.AuthCore.Model;

namespace ToolsToLive.AuthCore
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddAuthCore<TUser>(this IServiceCollection services, IConfigurationSection configurationSection) where TUser : IUser
        {
            services.Configure<AuthOptions>(configurationSection);

            services.AddSingleton<IIdentityService, IdentityService>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IAuthCoreService<TUser>, AuthCoreService<TUser>>();

            return services;
        }
    }
}
