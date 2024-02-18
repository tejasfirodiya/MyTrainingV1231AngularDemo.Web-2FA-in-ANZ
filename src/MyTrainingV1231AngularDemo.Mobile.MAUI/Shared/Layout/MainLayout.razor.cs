using Flurl.Http;
using MyTrainingV1231AngularDemo.ApiClient;
using MyTrainingV1231AngularDemo.Core.Dependency;
using Plugin.Connectivity;
using MyTrainingV1231AngularDemo.Services.Navigation;
using Microsoft.AspNetCore.Components;
using MyTrainingV1231AngularDemo.Mobile.MAUI.Services.Account;
using MyTrainingV1231AngularDemo.Mobile.MAUI.Services.UI;
using Microsoft.JSInterop;
using MyTrainingV1231AngularDemo.Mobile.MAUI.Services.Tenants;
using MyTrainingV1231AngularDemo.Mobile.MAUI.Helpers;

#if ANDROID
using MyTrainingV1231AngularDemo.Mobile.MAUI.Platforms.Android.HttpClient;
#endif

namespace MyTrainingV1231AngularDemo.Mobile.MAUI.Shared.Layout
{
    public partial class MainLayout
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected IJSRuntime JS { get; set; }

        protected UserDialogsService UserDialogsService { get; set; }

        private bool IsConfigurationsInitialized { get; set; }

        private string _logoURL;

        protected override async Task OnInitializedAsync()
        {
            UserDialogsService = DependencyResolver.Resolve<UserDialogsService>();
            UserDialogsService.Initialize(JS);

            await UserDialogsService.Block();

            await CheckInternetAndStartApplication();

            var navigationService = DependencyResolver.Resolve<INavigationService>();
            navigationService.Initialize(NavigationManager);

            _logoURL = await DependencyResolver.Resolve<TenantCustomizationService>().GetTenantLogo();

            await SetLayout();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await Task.Delay(200);
                await JS.InvokeVoidAsync("KTMenu.init");
            }
        }

        private async Task CheckInternetAndStartApplication()
        {
            if (CrossConnectivity.Current.IsConnected || ApiUrlConfig.IsLocal)
            {
                await StartApplication();
            }
            else
            {
                var isTryAgain = await UserDialogsService.Instance.Confirm(Localization.L.Localize("NoInternet"));
                if (!isTryAgain)
                {
                    CurrentApplicationCloser.Quit();
                }

                await CheckInternetAndStartApplication();
            }
        }

        private async Task StartApplication()
        {
            /*
              If you are using Genymotion Emulator, set DebugServerIpAddresses.Current = "10.0.3.2".
              If you are using a real Android device, set it as your computer's local IP and 
                 make sure your Android device and your computer is connecting to the internet via your local Wi-Fi.
           */
            DebugServerIpAddresses.Current = "10.0.2.2";

            ConfigureFlurlHttp();
            App.LoadPersistedSession();

            if (UserConfigurationManager.HasConfiguration)
            {
                IsConfigurationsInitialized = true;
                await UserDialogsService.UnBlock();

            }
            else
            {
                await UserConfigurationManager.GetAsync(async () =>
                {
                    IsConfigurationsInitialized = true;
                    await UserDialogsService.UnBlock();
                });
            }
        }

        private static void ConfigureFlurlHttp()
        {
#if ANDROID
            var abpHttpClientFactory = new AndroidHttpClientFactory
            {
                OnSessionTimeOut = App.OnSessionTimeout,
                OnAccessTokenRefresh = App.OnAccessTokenRefresh
            };
#elif IOS
            var abpHttpClientFactory = new MyTrainingV1231AngularDemo.Mobile.MAUI.Platforms.iOS.iOSHttpClientFactory
            {
                OnSessionTimeOut = App.OnSessionTimeout,
                OnAccessTokenRefresh = App.OnAccessTokenRefresh
            };
#endif
            FlurlHttp.Configure(c =>
            {
                c.HttpClientFactory = abpHttpClientFactory;
            });
        }

        private async Task SetLayout()
        {
            var dom = DependencyResolver.Resolve<DomManipulatorService>();
            await dom.ClearAllAttributes(JS, "body");
            await dom.SetAttribute(JS, "body", "id", "kt_app_body");
            await dom.SetAttribute(JS, "body", "data-kt-app-layout", "light-sidebar");
            await dom.SetAttribute(JS, "body", "data-kt-app-sidebar-enabled", "true");
            await dom.SetAttribute(JS, "body", "data-kt-app-sidebar-fixed", "true");
            await dom.SetAttribute(JS, "body", "data-kt-app-toolbar-enabled", "true");
            await dom.SetAttribute(JS, "body", "class", "app-default");
        }
    }
}
