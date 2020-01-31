using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ToolsToLive.AuthCore.Interfaces;
using ToolsToLive.AuthCore.Model;

namespace ToolsToLive.AuthCore
{
    public class AuthService<TUser> : IAuthService<TUser> where TUser : IUser
    {
        private readonly IOptions<AuthOptions> _options;
        private readonly IIdentityService _identityService;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserStore<TUser> _userStore;

        public AuthService(
            IOptions<AuthOptions> options,
            IIdentityService identityService,
            ITokenGenerator tokenGenerator,
            IPasswordHasher passwordHasher,
            IUserStore<TUser> userStore
            )
        {
            _options = options;
            _identityService = identityService;
            _tokenGenerator = tokenGenerator;
            _passwordHasher = passwordHasher;
            _userStore = userStore;
        }

        public ClaimsPrincipal Auth(string token)
        {
            var principal = _identityService.GetPrincipalFromToken(token);
            return principal;
        }

        public async Task<AuthResult<TUser>> CheckPasswordAndGenerateToken(string userNameOrEmail, string password)
        {
            TUser user = await _userStore.GetUserByUserName(userNameOrEmail);
            if (user == null)
            {
                user = await _userStore.GetUserByEmail(userNameOrEmail);
            }
            if (user == null)
            {
                return new AuthResult<TUser>(AuthResultType.UserNotFound);
            }

            if (!_passwordHasher.VerifyHashedPassword(user.PasswordHash, password))
            {
                return new AuthResult<TUser>(AuthResultType.PasswordIsWrong);
            }

            await _userStore.DeleteRefreshToken(user.UserName);

            IAuthToken token = await _tokenGenerator.GenerateToken(user);
            IAuthToken refreshToken = await _tokenGenerator.GenerateRefreshToken(user);
            await _userStore.SaveRefreshToken(refreshToken);

            await _userStore.UpdateLastActivity(user.Id);

            return PrepareAuthResult(user, token, refreshToken);
        }

        public async Task<AuthResult<TUser>> RefreshToken(string token, string refreshToken)
        {
            ClaimsPrincipal principal = _identityService.GetPrincipalFromToken(token);
            var username = principal.Identity.Name;

            TUser user = await _userStore.GetUserByUserName(username);

            if (user == null)
            {
                return new AuthResult<TUser>(AuthResultType.UserNotFound);
            }

            IAuthToken tokenInfo = await _tokenGenerator.GenerateToken(user);
            IAuthToken refreshTokenInfo = await _tokenGenerator.GenerateRefreshToken(user);
            await _userStore.UpdateRefreshToken(username, refreshTokenInfo.Token);

            await _userStore.UpdateLastActivity(user.Id);

            return PrepareAuthResult(user, tokenInfo, refreshTokenInfo);
        }

        /// <summary>
        /// Validate refresh token
        /// </summary>
        /// <returns>if token is valid - (true, null), otherwise (false, [reason])</returns>
        private async Task<(bool, AuthResult<TUser>)> ValidaateRefreshToken(string userName, string refreshTokenToVerify)
        {
            IAuthToken savedRefreshToken = await _userStore.GetRefreshToken(userName); //retrieve the refresh token from data storage

            if (savedRefreshToken == null
                || savedRefreshToken.Token != refreshTokenToVerify)
            {
                return (false, new AuthResult<TUser>(AuthResultType.RefreshTokenWrong));
            }

            if (savedRefreshToken.ExpireDate < DateTime.UtcNow)
            {
                return (false, new AuthResult<TUser>(AuthResultType.RefreshTokenExpired));
            }

            return (true, null);
        }

        private AuthResult<TUser> PrepareAuthResult(TUser user, IAuthToken token, IAuthToken refreshToken)
        {
            user.PasswordHash = null; //Do not show password hash to user

            return new AuthResult<TUser>(AuthResultType.Success)
            {
                Token = token,
                RefreshToken = refreshToken,
                User = user
            };
        }
    }
}
