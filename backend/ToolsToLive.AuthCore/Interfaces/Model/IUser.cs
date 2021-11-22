using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace ToolsToLive.AuthCore.Interfaces.Model
{
    public interface IUser
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
        string PasswordHash { get; set; }

        /// <summary>
        /// User's roles (will be added to token)
        /// </summary>
        IEnumerable<IRole> Roles { get; }

        /// <summary>
        /// Additional claims (will be added to token)
        /// </summary>
        IEnumerable<Claim> Claims { get; }

        DateTime? LockoutEndDate { get; set; }

        int AccessFailedCount { get; set; }
    }
}
