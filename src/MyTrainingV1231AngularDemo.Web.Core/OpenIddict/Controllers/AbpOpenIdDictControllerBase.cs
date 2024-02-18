using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using MyTrainingV1231AngularDemo.Authorization.Users;
using MyTrainingV1231AngularDemo.Identity;
using MyTrainingV1231AngularDemo.Web.OpenIddict.Claims;
using OpenIddict.Abstractions;

namespace MyTrainingV1231AngularDemo.Web.OpenIddict.Controllers
{
    public abstract class AbpOpenIdDictControllerBase : AbpController
    {
        protected readonly SignInManager SignInManager;
        protected readonly UserManager UserManager;
        protected readonly IOpenIddictApplicationManager ApplicationManager;
        protected readonly IOpenIddictAuthorizationManager AuthorizationManager;
        protected readonly IOpenIddictScopeManager ScopeManager;
        protected readonly IOpenIddictTokenManager TokenManager;
        protected readonly AbpOpenIddictClaimsPrincipalManager OpenIddictClaimsPrincipalManager;

        protected AbpOpenIdDictControllerBase(
            SignInManager signInManager, 
            UserManager userManager, 
            IOpenIddictApplicationManager applicationManager,
            IOpenIddictAuthorizationManager authorizationManager, 
            IOpenIddictScopeManager scopeManager, 
            IOpenIddictTokenManager tokenManager, 
            AbpOpenIddictClaimsPrincipalManager openIddictClaimsPrincipalManager)
        {
            SignInManager = signInManager;
            UserManager = userManager;
            ApplicationManager = applicationManager;
            AuthorizationManager = authorizationManager;
            ScopeManager = scopeManager;
            TokenManager = tokenManager;
            OpenIddictClaimsPrincipalManager = openIddictClaimsPrincipalManager;
            
            LocalizationSourceName = MyTrainingV1231AngularDemoConsts.LocalizationSourceName;
        }

        protected virtual Task<OpenIddictRequest> GetOpenIddictServerRequestAsync(HttpContext httpContext)
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                          throw new InvalidOperationException(L("TheOpenIDConnectRequestCannotBeRetrieved"));

            return Task.FromResult(request);
        }

        protected virtual async Task<IEnumerable<string>> GetResourcesAsync(ImmutableArray<string> scopes)
        {
            var resources = new List<string>();
            if (!scopes.Any())
            {
                return resources;
            }

            await foreach (var resource in ScopeManager.ListResourcesAsync(scopes))
            {
                resources.Add(resource);
            }

            return resources;
        }

        protected virtual async Task<bool> HasFormValueAsync(string name)
        {
            if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();
                if (!string.IsNullOrEmpty(form[name]))
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual async Task<bool> PreSignInCheckAsync(User user)
        {
            if (!await SignInManager.CanSignInAsync(user))
            {
                return false;
            }

            if (await UserManager.IsLockedOutAsync(user))
            {
                return false;
            }

            return true;
        }
    }
}