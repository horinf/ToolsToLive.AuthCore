using System;
using System.Collections.Generic;
using System.Security.Claims;
using Newtonsoft.Json;

namespace ToolsToLive.AuthCore.Interfaces.Model
{
    public interface IAuthCoreUser
    {
        /// <summary>
        /// User identifier (will be added to token)
        /// </summary>
        string Id { get; }

        /// <summary>
        /// User name (will be added to token)
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// Password hash (from/to database) (is used to save passwordHash to storage)
        /// </summary>
        [JsonIgnore]
        string PasswordHash { get; set; }

        /// <summary>
        /// User's roles (will be added to token)
        /// </summary>
        [JsonIgnore]
        List<string> Roles { get; }

        /// <summary>
        /// Additional claims (will be added to token)
        /// </summary>
        [JsonIgnore]
        List<Claim> Claims { get; }

        [JsonIgnore]
        DateTime? LockoutEndDate { get; set; }

        [JsonIgnore]
        int AccessFailedCount { get; set; }
    }
}
