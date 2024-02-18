using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Abp;
using Abp.UI;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Runtime.Caching;
using Abp.Runtime.Security;
using Abp.Timing;
using Microsoft.IdentityModel.Tokens;
using MyTrainingV1231AngularDemo.Authorization.Users;
using MyTrainingV1231AngularDemo.Authorization.Delegation;
using MyTrainingV1231AngularDemo.Authorization;

namespace MyTrainingV1231AngularDemo.Web.Authentication.JwtBearer
{
    public class MyTrainingV1231AngularDemoAsyncJwtSecurityTokenHandler : IAsyncSecurityTokenValidator
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public MyTrainingV1231AngularDemoAsyncJwtSecurityTokenHandler()
        {
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public bool CanValidateToken => true;

        public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

        public bool CanReadToken(string securityToken)
        {
            return _tokenHandler.CanReadToken(securityToken);
        }

        public async Task<(ClaimsPrincipal, SecurityToken)> ValidateToken(string securityToken,
            TokenValidationParameters validationParameters)
        {
            var principal = _tokenHandler.ValidateToken(securityToken, validationParameters, out var validatedToken);

            if (!HasTokenType(principal, TokenType.AccessToken))
            {
                throw new SecurityTokenException("invalid token type");
            }

            return await ValidateTokenInternal(principal, validatedToken);
        }

        public async Task<(ClaimsPrincipal, SecurityToken)> ValidateRefreshToken(string securityToken,
            TokenValidationParameters validationParameters)
        {
            var principal = _tokenHandler.ValidateToken(securityToken, validationParameters, out var validatedToken);

            if (!HasTokenType(principal, TokenType.RefreshToken))
            {
                throw new SecurityTokenException("invalid token type");
            }

            return await ValidateTokenInternal(principal, validatedToken);
        }

        private async Task<(ClaimsPrincipal, SecurityToken)> ValidateTokenInternal(ClaimsPrincipal principal,
            SecurityToken validatedToken)
        {
            var cacheManager = IocManager.Instance.Resolve<ICacheManager>();
            await ValidateSecurityStampAsync(principal);

            var tokenValidityKeyClaim = principal.Claims.First(c => c.Type == AppConsts.TokenValidityKey);
            if (await TokenValidityKeyExistsInCache(tokenValidityKeyClaim, cacheManager))
            {
                return (principal, validatedToken);
            }

            var userIdentifierString = principal.Claims.First(c => c.Type == AppConsts.UserIdentifier);
            var userIdentifier = UserIdentifier.Parse(userIdentifierString.Value);

            if (!await ValidateTokenValidityKey(tokenValidityKeyClaim, userIdentifier))
            {
                throw new SecurityTokenException("invalid");
            }

            var tokenAuthConfiguration = IocManager.Instance.Resolve<TokenAuthConfiguration>();

            await cacheManager.GetCache(AppConsts.TokenValidityKey).SetAsync(
                tokenValidityKeyClaim.Value, "",
                absoluteExpireTime: new DateTimeOffset(
                    Clock.Now.AddMinutes(tokenAuthConfiguration.AccessTokenExpiration.TotalMinutes)
                )
            );

            return (principal, validatedToken);
        }

        private async Task<bool> ValidateTokenValidityKey(Claim tokenValidityKeyClaim, UserIdentifier userIdentifier)
        {
            bool isValid;

            using (var unitOfWorkManager = IocManager.Instance.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (var uow = unitOfWorkManager.Object.Begin())
                {
                    using (unitOfWorkManager.Object.Current.SetTenantId(userIdentifier.TenantId))
                    {
                        using (var userManager = IocManager.Instance.ResolveAsDisposable<UserManager>())
                        {
                            var userManagerObject = userManager.Object;
                            var user = await userManagerObject.GetUserAsync(userIdentifier);
                            isValid = await userManagerObject.IsTokenValidityKeyValidAsync(
                                user,
                                tokenValidityKeyClaim.Value
                            );

                            await uow.CompleteAsync();
                        }
                    }
                }
            }

            return isValid;
        }

        private static async Task<bool> TokenValidityKeyExistsInCache(Claim tokenValidityKeyClaim,
            ICacheManager cacheManager)
        {
            var tokenValidityKeyInCache = await cacheManager
                .GetCache(AppConsts.TokenValidityKey)
                .GetOrDefaultAsync(tokenValidityKeyClaim.Value);

            return tokenValidityKeyInCache != null;
        }

        private static async Task ValidateSecurityStampAsync(ClaimsPrincipal principal)
        {
            ValidateUserDelegation(principal);

            using (var securityStampHandler = IocManager.Instance.ResolveAsDisposable<IJwtSecurityStampHandler>())
            {
                if (!await securityStampHandler.Object.Validate(principal))
                {
                    throw new SecurityTokenException("invalid");
                }
            }
        }

        private bool HasTokenType(ClaimsPrincipal principal, TokenType tokenType)
        {
            return principal.Claims.FirstOrDefault(x => x.Type == AppConsts.TokenType)?.Value ==
                   tokenType.To<int>().ToString();
        }

        private static void ValidateUserDelegation(ClaimsPrincipal principal)
        {
            var userDelegationConfiguration = IocManager.Instance.Resolve<IUserDelegationConfiguration>();
            if (!userDelegationConfiguration.IsEnabled)
            {
                return;
            }

            var impersonatorTenant = principal.Claims.FirstOrDefault(c => c.Type == AbpClaimTypes.ImpersonatorTenantId);
            var user = principal.Claims.FirstOrDefault(c => c.Type == AbpClaimTypes.UserId);
            var impersonatorUser = principal.Claims.FirstOrDefault(c => c.Type == AbpClaimTypes.ImpersonatorUserId);

            if (impersonatorUser == null || user == null)
            {
                return;
            }

            var impersonatorTenantId = impersonatorTenant == null ? null :
                impersonatorTenant.Value.IsNullOrEmpty() ? (int?)null : Convert.ToInt32(impersonatorTenant.Value);
            var sourceUserId = Convert.ToInt64(user.Value);
            var impersonatorUserId = Convert.ToInt64(impersonatorUser.Value);

            using (var permissionChecker = IocManager.Instance.ResolveAsDisposable<PermissionChecker>())
            {
                if (permissionChecker.Object.IsGranted(
                        new UserIdentifier(impersonatorTenantId, impersonatorUserId),
                        AppPermissions.Pages_Administration_Users_Impersonation)
                   )
                {
                    return;
                }
            }

            using (var userDelegationManager = IocManager.Instance.ResolveAsDisposable<IUserDelegationManager>())
            {
                var hasActiveDelegation = userDelegationManager.Object.HasActiveDelegation(
                    sourceUserId,
                    impersonatorUserId
                );

                if (!hasActiveDelegation)
                {
                    throw new UserFriendlyException("ThereIsNoActiveUserDelegationBetweenYourUserAndCurrentUser");
                }
            }
        }
    }
}