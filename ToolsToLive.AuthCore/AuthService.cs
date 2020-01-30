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

            IAuthToken tokenInfo = CreateToken(user);
            IAuthToken refreshTokenInfo = await CreateAndSaveRefreshToken(user);

            await _userStore.UpdateLastActivity(user.Id);

            return PrepareAuthResult(user, tokenInfo, refreshTokenInfo);
        }

        public async Task<AuthResult<TUser>> RefreshToken(string token, string refreshToken)
        {
            ClaimsPrincipal principal = _identityService.GetPrincipalFromToken(token);
            var username = principal.Identity.Name;

            IAuthToken savedRefreshToken = await _userStore.GetRefreshToken(username); //retrieve the refresh token from data storage

            if (savedRefreshToken == null
                || savedRefreshToken.Token != refreshToken)
            {
                return new AuthResult<TUser>(AuthResultType.RefreshTokenWrong);
            }

            if (savedRefreshToken.ExpireDate < DateTime.UtcNow)
            {
                return new AuthResult<TUser>(AuthResultType.RefreshTokenExpired);
            }

            TUser user = await _userStore.GetUserByUserName(username);

            if (user == null)
            {
                return new AuthResult<TUser>(AuthResultType.UserNotFound);
            }

            IAuthToken tokenInfo = CreateToken(user);
            IAuthToken refreshTokenInfo = await UpdateAndSaveRefreshToken(savedRefreshToken);

            await _userStore.UpdateLastActivity(user.Id);

            return PrepareAuthResult(user, tokenInfo, refreshTokenInfo);
        }

        private async Task<IAuthToken> CreateAndSaveRefreshToken(IUser user)
        {
            var now = DateTime.UtcNow;
            var expires = now.Add(_options.Value.RefreshTokenLifeTime);

            string refreshToken = _tokenGenerator.GenerateRefreshToken();
            var authRefreshToken = new AuthToken
            {
                Token = refreshToken,
                IssueDate = now,
                ExpireDate = expires,
                UserName = user.UserName,
            };

            await _userStore.SaveRefreshToken(authRefreshToken);

            return authRefreshToken;
        }

        private AuthResult<TUser> PrepareAuthResult(TUser user, IAuthToken tokenInfo, IAuthToken refreshTokenInfo)
        {
            user.PasswordHash = null; //Do not show password hash to user

            return new AuthResult<TUser>(AuthResultType.Success)
            {
                Token = tokenInfo,
                RefreshToken = refreshTokenInfo,
                User = user
            };
        }

        private IAuthToken CreateToken(IUser user)
        {
            DateTime now = DateTime.UtcNow;
            DateTime expires = now.Add(_options.Value.TokenLifetime);

            ClaimsIdentity identity = _identityService.GetIdentity(user);
            string token = _tokenGenerator.GenerateToken(identity, now, expires);

            return new AuthToken
            {
                Token = token,
                IssueDate = now,
                ExpireDate = expires,
                UserName = user.UserName,
            };
        }

        private async Task<IAuthToken> UpdateAndSaveRefreshToken(IAuthToken savedRefreshToken)
        {
            string refreshToken = _tokenGenerator.GenerateRefreshToken();
            await _userStore.UpdateRefreshToken(savedRefreshToken.UserName, refreshToken);

            savedRefreshToken.Token = refreshToken;
            return savedRefreshToken;
        }

    }
}
