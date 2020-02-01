using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToolsToLive.AuthCore.Interfaces;
using ToolsToLive.AuthCore.Interfaces.Model;
using ToolsToLive.AuthCore.Model;

namespace ToolsToLive.AuthCore
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Sets up dependency for using auth core, including storage, auth options etc.
        /// (storage is setting up as scoped service).
        /// </summary>
        /// <typeparam name="TUser">Type of the user in application.</typeparam>
        /// <typeparam name="TUserStorage">Type of the storrage to set up IUserStorage dependency.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="configurationSection">Configuration section that should match to <see cref="AuthOptions"/> class.</param>
        /// <returns></returns>
        public static IServiceCollection AddAuthCore<TUser, TUserStorage>(this IServiceCollection services, IConfigurationSection configurationSection) where TUser : IUser where TUserStorage : class, IUserStorage<TUser>
        {
            services.Configure<AuthOptions>(configurationSection);

            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IAuthCoreService<TUser>, AuthCoreService<TUser>>();
            services.AddScoped<IUserStorage<TUser>, TUserStorage>();

            return services;
        }
    }
}
