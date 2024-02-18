using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyTrainingV1231AngularDemo.Authorization.Roles;
using MyTrainingV1231AngularDemo.Authorization.Users;
using MyTrainingV1231AngularDemo.Configuration;
using MyTrainingV1231AngularDemo.MultiTenancy;

namespace MyTrainingV1231AngularDemo.Identity
{
    public class SignInManager : AbpSignInManager<Tenant, Role, User>
    {
        private readonly ISettingManager _settingManager;
        private readonly IAbpSession _abpSession;

        public SignInManager(
            UserManager userManager,
            IHttpContextAccessor contextAccessor,
            UserClaimsPrincipalFactory claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<User>> logger,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<User> userConfirmation,
            IAbpSession abpSession) 
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, unitOfWorkManager, settingManager, schemes, userConfirmation)
        {
            _settingManager = settingManager;
            _abpSession = abpSession;
        }

        public override async Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
        {
            var schemes = await base.GetExternalAuthenticationSchemesAsync();
            
            if (_abpSession.TenantId.HasValue)
            {
                schemes = schemes.Where(IsSchemeEnabledOnTenant);
            }

            return schemes;
        }

        private bool IsSchemeEnabledOnTenant(AuthenticationScheme scheme)
        {
            switch (scheme.Name)
            {
                case "OpenIdConnect":
                    return !_settingManager.GetSettingValueForTenant<bool>(AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect_IsDeactivated,_abpSession.GetTenantId());
                case "Microsoft": 
                    return !_settingManager.GetSettingValueForTenant<bool>(AppSettings.ExternalLoginProvider.Tenant.Microsoft_IsDeactivated,_abpSession.GetTenantId());
                case "Google": 
                    return !_settingManager.GetSettingValueForTenant<bool>(AppSettings.ExternalLoginProvider.Tenant.Google_IsDeactivated,_abpSession.GetTenantId());
                case "Twitter": 
                    return !_settingManager.GetSettingValueForTenant<bool>(AppSettings.ExternalLoginProvider.Tenant.Twitter_IsDeactivated,_abpSession.GetTenantId());
                case "Facebook":
                    return !_settingManager.GetSettingValueForTenant<bool>(AppSettings.ExternalLoginProvider.Tenant.Facebook_IsDeactivated,_abpSession.GetTenantId());
                case "WsFederation": 
                    return !_settingManager.GetSettingValueForTenant<bool>(AppSettings.ExternalLoginProvider.Tenant.WsFederation_IsDeactivated,_abpSession.GetTenantId());
                default: return true;
            }
        }
    }
}
