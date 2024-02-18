using Abp.Dependency;
using Abp.Extensions;
using MyTrainingV1231AngularDemo.ApiClient;
using MyTrainingV1231AngularDemo.Tenants;

namespace MyTrainingV1231AngularDemo.Mobile.MAUI.Services.Tenants
{
    public class TenantCustomizationService : ITenantCustomizationService, ITransientDependency
    {
        private const string DefaultLogo = "media/logos/app-logo-on-light.svg";

        private ProxyTenantCustomizationControllerService _proxyTenantCustomizationControllerService;
        private IApplicationContext _applicationContext;

        private int _lastRequestedLogoTenantId;
        private string _lastRequestedLogo;

        public TenantCustomizationService(ProxyTenantCustomizationControllerService proxyTenantCustomizationControllerService, IApplicationContext applicationContext)
        {
            _proxyTenantCustomizationControllerService = proxyTenantCustomizationControllerService;
            _applicationContext = applicationContext;
        }

        private bool HasTenant => (_applicationContext.CurrentTenant?.TenantId ?? null) != null;

        private bool HasCachedLogo(int tenantId) => _lastRequestedLogoTenantId == tenantId && !_lastRequestedLogo.IsNullOrWhiteSpace();

        public async Task<string> GetTenantLogo()
        {
            if (!HasTenant)
            {
                return DefaultLogo;
            }

            if (HasCachedLogo(_applicationContext.CurrentTenant.TenantId))
            {
                return _lastRequestedLogo;
            }
            else
            {
                _lastRequestedLogo = null;
                _lastRequestedLogoTenantId = -1;
            }

            var getLogoOutput = await _proxyTenantCustomizationControllerService.GetTenantLogoOrNull(_applicationContext.CurrentTenant.TenantId);
            if (getLogoOutput.HasLogo)
            {
                _lastRequestedLogoTenantId = _applicationContext.CurrentTenant.TenantId;
                _lastRequestedLogo = "data:" + getLogoOutput.LogoFileType + ";base64, " + getLogoOutput.Logo;
                return _lastRequestedLogo;
            }

            return DefaultLogo;
        }
    }
}
