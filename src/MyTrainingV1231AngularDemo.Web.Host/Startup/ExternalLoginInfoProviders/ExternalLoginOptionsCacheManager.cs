using Abp.AspNetZeroCore.Web.Authentication.External.Facebook;
using Abp.AspNetZeroCore.Web.Authentication.External.Google;
using Abp.AspNetZeroCore.Web.Authentication.External.Microsoft;
using Abp.AspNetZeroCore.Web.Authentication.External.OpenIdConnect;
using Abp.AspNetZeroCore.Web.Authentication.External.Twitter;
using Abp.AspNetZeroCore.Web.Authentication.External.WsFederation;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using MyTrainingV1231AngularDemo.Configuration;

namespace MyTrainingV1231AngularDemo.Web.Startup.ExternalLoginInfoProviders
{
    public class ExternalLoginOptionsCacheManager : IExternalLoginOptionsCacheManager, ITransientDependency
    {
        private readonly ICacheManager _cacheManager;
        private readonly IAbpSession _abpSession;

        public ExternalLoginOptionsCacheManager(ICacheManager cacheManager, IAbpSession abpSession)
        {
            _cacheManager = cacheManager;
            _abpSession = abpSession;
        }

        public void ClearCache()
        {
            _cacheManager.GetExternalLoginInfoProviderCache().Remove(GetCacheKey(FacebookAuthProviderApi.Name));
            _cacheManager.GetExternalLoginInfoProviderCache().Remove(GetCacheKey(GoogleAuthProviderApi.Name));
            _cacheManager.GetExternalLoginInfoProviderCache().Remove(GetCacheKey(TwitterAuthProviderApi.Name));
            _cacheManager.GetExternalLoginInfoProviderCache().Remove(GetCacheKey(MicrosoftAuthProviderApi.Name));
            _cacheManager.GetExternalLoginInfoProviderCache().Remove(GetCacheKey(OpenIdConnectAuthProviderApi.Name));
            _cacheManager.GetExternalLoginInfoProviderCache().Remove(GetCacheKey(WsFederationAuthProviderApi.Name));
        }

        private string GetCacheKey(string name)
        {
            if (_abpSession.TenantId.HasValue)
            {
                return $"{name}-{_abpSession.TenantId.Value}";
            }

            return $"{name}";
        }
    }
}