using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ToolsToLive.AuthCore.Interfaces;
using ToolsToLive.AuthCore.Interfaces.Helpers;
using ToolsToLive.AuthCore.Interfaces.IdentityServices;
using ToolsToLive.AuthCore.Interfaces.Model;
using ToolsToLive.AuthCore.Interfaces.Storage;
using ToolsToLive.AuthCore.Model;

namespace ToolsToLive.AuthCore
{
    public class AuthCoreService<TUser> : IAuthCoreService<TUser> where TUser : IAuthCoreUser
    {
        private readonly IUserStorageService<TUser> _userStore;
        private readonly IRefreshTokenStorageService _refreshTokenStorageService;
        private readonly IIdentityService _identityService;
        private readonly IOptions<AuthOptions> _options;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthCookiesHelper _authCookiesHelper;
        private readonly IIdentityValidationService _identityValidationService;

        public AuthCoreService(
            IUserStorageService<TUser> userStore,
            IRefreshTokenStorageService refreshTokenStorageService,
            IIdentityService identityService,
            IOptions<AuthOptions> options,
            IPasswordHasher passwordHasher,
            IAuthCookiesHelper authCookiesHelper,
            IIdentityValidationService identityValidationService)
        {
            _userStore = userStore;
            _refreshTokenStorageService = refreshTokenStorageService;
            _identityService = identityService;
            _options = options;
            _passwordHasher = passwordHasher;
            _authCookiesHelper = authCookiesHelper;
            _identityValidationService = identityValidationService;
        }

        public ClaimsPrincipal GetPrincipalByToken(string token)
        {
            var principal = _identityValidationService.GetPrincipalFromToken(token);
            return principal;
        }

        public async Task<AuthResult<TUser>> SignIn(string userNameOrEmail, string password, string deviceId, IResponseCookies responseCookies)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                throw new ArgumentNullException(nameof(deviceId));
            }

            // an attempt to get user from db (by user name or email)
            var user = await _userStore.GetUserByUserNameOrEmail(userNameOrEmail);

            if (user == null)
            {
                return new AuthResult<TUser>(AuthResultType.UserNotFound);
            }

            if (user.LockoutEndDate.HasValue && user.LockoutEndDate > DateTime.UtcNow)
            {
                return new AuthResult<TUser>(AuthResultType.LockedOut);
            }

            // password verification
            if (!_passwordHasher.VerifyPassword(user.PasswordHash, password))
            {
                user.AccessFailedCount++;
                if (user.AccessFailedCount > _options.Value.MaxAccessFailedCount)
                {
                    user.LockoutEndDate = DateTime.UtcNow.Add(_options.Value.LockoutPeriod);
                }
                await _userStore.UpdateLockoutData(user.Id, user.AccessFailedCount, user.LockoutEndDate);

                return new AuthResult<TUser>(AuthResultType.PasswordWrong);
            }

            if (user.AccessFailedCount > 0)
            {
                await _userStore.UpdateLockoutData(user.Id, 0, null);
            }

            // User found, password correct
            return await SignIn(user, deviceId, responseCookies);
        }

        public async Task<AuthResult<TUser>> SignIn(string userId, string deviceId, IResponseCookies responseCookies)
        {
            // retreive user from db
            var user = await _userStore.GetUserById(userId);

            if (user == null)
            {
                return new AuthResult<TUser>(AuthResultType.UserNotFound);
            }

            return await SignIn(user, deviceId, responseCookies);
        }

        private async Task<AuthResult<TUser>> SignIn(TUser user, string deviceId, IResponseCookies responseCookies)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                throw new ArgumentNullException(nameof(deviceId));
            }

            var accessToken = _identityService.GenerateAuthToken(user);
            var refreshToken = _identityService.GenerateRefreshToken(user);

            await _refreshTokenStorageService.SaveNewRefreshToken(refreshToken, deviceId);
            await _userStore.UpdateLastActivity(user.Id);

            // Mobile apps can't use cookies, but they can store device id by themselves.
            _authCookiesHelper.SetDeviceIdCookie(responseCookies, deviceId, refreshToken.ExpireDate);

            if (_options.Value.AddTokenToCookie)
            {
                var cookieToken = _identityService.GenerateAuthTokenForCookie(user);
                _authCookiesHelper.SetAuthCookie(responseCookies, cookieToken.Token, cookieToken.ExpireDate);
            }

            return PrepareAuthResult(user, accessToken.Token, refreshToken);
        }

        public async Task SignOut(string userId, string deviceId, IRequestCookieCollection requestCookies, IResponseCookies responseCookies)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                deviceId = _authCookiesHelper.GetDeviceIdFromCookie(requestCookies);
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentNullException(nameof(deviceId));
            }

            _authCookiesHelper.ClearDeviceIdCookie(responseCookies);
            _authCookiesHelper.ClearAuthCookie(responseCookies);

            await _refreshTokenStorageService.DeleteRefreshToken(userId, deviceId);
        }

        public async Task SignOutFromEverywhere(string userId, IResponseCookies responseCookies)
        {
            _authCookiesHelper.ClearDeviceIdCookie(responseCookies);
            _authCookiesHelper.ClearAuthCookie(responseCookies);

            await _refreshTokenStorageService.DeleteRefreshTokens(userId);
        }

        public async Task<AuthResult<TUser>> RefreshToken(string userId, string deviceId, string providedRefreshToken, IRequestCookieCollection requestCookies, IResponseCookies responseCookies)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                deviceId = _authCookiesHelper.GetDeviceIdFromCookie(requestCookies);
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (string.IsNullOrEmpty(deviceId))
            {
                return new AuthResult<TUser>(AuthResultType.RefreshTokenWrong);
            }

            // retreive user from db
            var user = await _userStore.GetUserById(userId);
            if (user == null)
            {
                return new AuthResult<TUser>(AuthResultType.UserNotFound);
            }

            //verify refresh token
            var verRefreshToken = await VerifyRefreshToken(userId, deviceId, providedRefreshToken);
            if (!verRefreshToken)
            {
                return new AuthResult<TUser>(AuthResultType.RefreshTokenWrong);
            }

            // create token and refresh token
            var accessToken = _identityService.GenerateAuthToken(user);
            var refreshToken = _identityService.GenerateRefreshToken(user);

            await _refreshTokenStorageService.UpdateRefreshToken(userId, deviceId, refreshToken);
            await _userStore.UpdateLastActivity(user.Id);

            // Mobile apps can't use cookies, but they can store device id by themselves.
            _authCookiesHelper.SetDeviceIdCookie(responseCookies, deviceId, refreshToken.ExpireDate);

            if (_options.Value.AddTokenToCookie)
            {
                var cookieToken = _identityService.GenerateAuthTokenForCookie(user);
                _authCookiesHelper.SetAuthCookie(responseCookies, cookieToken.Token, cookieToken.ExpireDate);
            }

            return PrepareAuthResult(user, accessToken.Token, refreshToken);
        }

        /// <summary>
        /// Validate refresh token
        /// </summary>
        private async Task<bool> VerifyRefreshToken(string userId, string deviceId, string refreshTokenToVerify)
        {
            var savedRefreshToken = await _refreshTokenStorageService.GetRefreshToken(userId, deviceId); //retrieve the refresh token from data storage

            if (savedRefreshToken != null &&
                savedRefreshToken.ExpireDate > DateTime.UtcNow)
            {
                if (savedRefreshToken.Token == refreshTokenToVerify)
                {
                    return true;
                }
                else
                {
                    // Soft grace period - previous token still valid for a few seconds
                    var nowDiff = DateTime.UtcNow.AddSeconds(-10);
                    if (savedRefreshToken.PreviousToken == refreshTokenToVerify &&
                        savedRefreshToken.IssueDate > nowDiff)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private AuthResult<TUser> PrepareAuthResult(TUser user, string accessToken, RefreshToken refreshToken)
        {
            user.PasswordHash = null; //Do not show password hash to user (it should have attribute JsonIgnore, but just in case)

            return new AuthResult<TUser>(AuthResultType.Success)
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = user
            };
        }
    }
}
