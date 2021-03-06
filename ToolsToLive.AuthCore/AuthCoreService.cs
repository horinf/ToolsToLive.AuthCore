﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToolsToLive.AuthCore.Interfaces;
using ToolsToLive.AuthCore.Interfaces.Model;
using ToolsToLive.AuthCore.Model;

namespace ToolsToLive.AuthCore
{
    public class AuthCoreService<TUser> : IAuthCoreService<TUser> where TUser : IUser
    {
        private readonly IIdentityService _identityService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserStorage<TUser> _userStore;

        public AuthCoreService(
            IIdentityService identityService,
            IPasswordHasher passwordHasher,
            IUserStorage<TUser> userStore
            )
        {
            _identityService = identityService;
            _passwordHasher = passwordHasher;
            _userStore = userStore;
        }

        //public ClaimsPrincipal Auth(string token)
        //{
        //    var principal = _identityService.GetPrincipalFromToken(token);
        //    return principal;
        //}

        public async Task SignOut(string userName, string sessionId)
        {
            await _userStore.DeleteRefreshToken(userName, sessionId);
        }

        public async Task SignOutFromEverywhere(string userName)
        {
            IEnumerable<IAuthToken> rTokens = await _userStore.GetRefreshTokens(userName);
            foreach (IAuthToken rToken in rTokens)
            {
                await _userStore.DeleteRefreshToken(rToken.UserName, rToken.SessionId);

            }
        }

        public async Task<AuthResult<TUser>> SignIn(string userNameOrEmail, string password)
        {
            // an attempt to get user from db (by user name or email)
            TUser user = await _userStore.GetUserByUserName(userNameOrEmail);
            if (user == null)
            {
                user = await _userStore.GetUserByEmail(userNameOrEmail);
            }
            if (user == null)
            {
                return new AuthResult<TUser>(AuthResultType.UserNotFound);
            }

            // password verification
            if (!_passwordHasher.VerifyHashedPassword(user.PasswordHash, password))
            {
                return new AuthResult<TUser>(AuthResultType.PasswordIsWrong);
            }

            // check if user is confirmed
            if (!user.IsConfirmed)
            {
                return new AuthResult<TUser>(AuthResultType.NotConfirmed);
            }

            // User found, password correct, user is confirmed

            string sessionId = Guid.NewGuid().ToString();

            IAuthToken token = await _identityService.GenerateToken(user, sessionId);
            IAuthToken refreshToken = await _identityService.GenerateRefreshToken(user, sessionId);

            await _userStore.SaveNewRefreshToken(refreshToken);
            await _userStore.UpdateLastActivity(user.Id);

            return PrepareAuthResult(user, token, refreshToken);
        }

        public async Task<AuthResult<TUser>> RefreshToken(string userName, string sessionId, string currentRefreshToken)
        {
            if (userName is null)
            {
                throw new ArgumentNullException(nameof(userName));
            }

            //verify refresh token
            bool verRefreshToken = await ValidaateRefreshToken(userName, sessionId, currentRefreshToken);
            if (!verRefreshToken)
            {
                return new AuthResult<TUser>(AuthResultType.RefreshTokenWrong);
            }

            // retreive user from db
            TUser user = await _userStore.GetUserByUserName(userName);

            if (user == null)
            {
                return new AuthResult<TUser>(AuthResultType.UserNotFound);
            }

            // create token and refresh token
            IAuthToken token = await _identityService.GenerateToken(user, sessionId);
            IAuthToken refreshToken = await _identityService.GenerateRefreshToken(user, sessionId);

            await _userStore.UpdateRefreshToken(userName, sessionId, refreshToken.Token);
            await _userStore.UpdateLastActivity(user.Id);

            return PrepareAuthResult(user, token, refreshToken);
        }

        /// <summary>
        /// Validate refresh token
        /// </summary>
        /// <returns>if token is valid - (true, null), otherwise (false, [reason])</returns>
        private async Task<bool> ValidaateRefreshToken(string userName, string sessionId, string refreshTokenToVerify)
        {
            IAuthToken savedRefreshToken = await _userStore.GetRefreshToken(userName, sessionId); //retrieve the refresh token from data storage

            if (savedRefreshToken == null
                || savedRefreshToken.Token != refreshTokenToVerify
                || savedRefreshToken.ExpireDate < DateTime.UtcNow)
            {
                return false;
            }

            return true;
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
