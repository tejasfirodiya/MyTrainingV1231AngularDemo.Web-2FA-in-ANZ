using MyTrainingV1231AngularDemo.ApiClient;
using MyTrainingV1231AngularDemo.Core.Dependency;
using MyTrainingV1231AngularDemo.Services.Account;
using MyTrainingV1231AngularDemo.Services.Navigation;
using MyTrainingV1231AngularDemo.Services.Storage;

namespace MyTrainingV1231AngularDemo.Mobile.MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        public static async Task OnSessionTimeout()
        {
            await DependencyResolver.Resolve<IAccountService>().LogoutAsync();
            DependencyResolver.Resolve<INavigationService>().NavigateTo(NavigationUrlConsts.Login);
        }

        public static async Task OnAccessTokenRefresh(string newAccessToken, string newEncryptedAccessToken)
        {
            await DependencyResolver.Resolve<IDataStorageService>().StoreAccessTokenAsync(newAccessToken, newEncryptedAccessToken);
        }

        public static void LoadPersistedSession()
        {
            var accessTokenManager = DependencyResolver.Resolve<IAccessTokenManager>();
            var dataStorageService = DependencyResolver.Resolve<IDataStorageService>();
            var applicationContext = DependencyResolver.Resolve<IApplicationContext>();

            accessTokenManager.AuthenticateResult = dataStorageService.RetrieveAuthenticateResult();
            applicationContext.Load(dataStorageService.RetrieveTenantInfo(), dataStorageService.RetrieveLoginInfo());
        }
    }
}