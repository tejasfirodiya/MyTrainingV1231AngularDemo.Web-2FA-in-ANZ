using MyTrainingV1231AngularDemo.Core.Dependency;
using MyTrainingV1231AngularDemo.Mobile.MAUI.Shared;
using MyTrainingV1231AngularDemo.Services.Account;
using MyTrainingV1231AngularDemo.Services.Navigation;

namespace MyTrainingV1231AngularDemo.Mobile.MAUI.Pages.MySettings
{
    public partial class Settings : MyTrainingV1231AngularDemoMainLayoutPageComponentBase
    {
        protected IAccountService AccountService { get; set; }
        protected NavMenu NavMenu { get; set; }

        protected INavigationService navigationService { get; set; }
        ChangePasswordModal changePasswordModal;

        public Settings()
        {
            AccountService = DependencyResolver.Resolve<IAccountService>();
            navigationService = DependencyResolver.Resolve<INavigationService>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPageHeader(L("MySettings"));
        }

        private async Task LogOut()
        {
            await AccountService.LogoutAsync();
            navigationService.NavigateTo(NavigationUrlConsts.Login);
        }

        private async Task OnChangePasswordAsync()
        {
            await changePasswordModal.Hide();
            await Task.Delay(300);
            await LogOut();
        }

        private async Task OnLanguageSwitchAsync()
        {
            await SetPageHeader(L("MySettings"));
            StateHasChanged();
        }

        private async Task ChangePassword()
        {
            await changePasswordModal.Show();
        }

    }
}
