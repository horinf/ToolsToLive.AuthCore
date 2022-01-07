using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToolsToLive.AuthCore.Helpers;
using ToolsToLive.AuthCore.IdentityServices;
using ToolsToLive.AuthCore.Interfaces;
using ToolsToLive.AuthCore.Interfaces.Helpers;
using ToolsToLive.AuthCore.Interfaces.IdentityServices;
using ToolsToLive.AuthCore.Interfaces.Model;
using ToolsToLive.AuthCore.Interfaces.Storage;
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
        public static IServiceCollection AddAuthCore<TUser, TUserStorageService, TRefreshTokenStorageService>(this IServiceCollection services, IConfigurationSection configurationSection)
            where TUser : IAuthCoreUser
            where TUserStorageService : class, IUserStorageService<TUser>
            where TRefreshTokenStorageService : class, IRefreshTokenStorageService
        {
            if (string.IsNullOrWhiteSpace(configurationSection[nameof(AuthOptions.Key)]))
            {
                throw new ArgumentException("Key must be set in configuration", nameof(configurationSection));
            }

            if (string.IsNullOrWhiteSpace(configurationSection[nameof(AuthOptions.CookieDomain)]))
            {
                throw new ArgumentException("CookieDomain must be set in configuration", nameof(configurationSection));
            }

            services.Configure<AuthOptions>(configurationSection);

            services.AddSingleton<ISigningCredentialsProvider, SigningCredentialsProvider>();
            services.AddSingleton<IIdentityValidationService, IdentityValidationService>();

            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IIdentityProvider, IdentityProvider>();
            services.AddSingleton<IIdentityService, IdentityService>();
            services.AddSingleton<ICodeGenerator, CodeGenerator>();

            services.AddScoped<IAuthCoreService<TUser>, AuthCoreService<TUser>>();
            services.AddScoped<IUserStorageService<TUser>, TUserStorageService>();
            services.AddScoped<IRefreshTokenStorageService, TRefreshTokenStorageService>();

            services.AddScoped<IAuthCookiesHelper, AuthCookiesHelper>();

            return services;
        }
    }
}
