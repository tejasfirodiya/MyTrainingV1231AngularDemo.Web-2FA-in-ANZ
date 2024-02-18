using MyTrainingV1231AngularDemo.ApiClient;
using MyTrainingV1231AngularDemo.Authorization.Accounts;
using MyTrainingV1231AngularDemo.Authorization.Accounts.Dto;
using MyTrainingV1231AngularDemo.Core.Dependency;
using MyTrainingV1231AngularDemo.Core.Threading;
using MyTrainingV1231AngularDemo.Mobile.MAUI.Services.UI;
using MyTrainingV1231AngularDemo.Mobile.MAUI.Shared;
using MyTrainingV1231AngularDemo.Services.Account;

namespace MyTrainingV1231AngularDemo.Mobile.MAUI.Pages.Login
{
    public partial class Index : MyTrainingV1231AngularDemoComponentBase
    {
        public string UserName
        {
            get => _accountService.AbpAuthenticateModel.UserNameOrEmailAddress;
            set
            {
                _accountService.AbpAuthenticateModel.UserNameOrEmailAddress = value;
            }
        }

        public string Password
        {
            get => _accountService.AbpAuthenticateModel.Password;
            set
            {
                _accountService.AbpAuthenticateModel.Password = value;
            }
        }

        private IAccountService _accountService;
        private IAccountAppService _accountAppService;
        private IApplicationContext _applicationContext;

        SwitchTenantModal switchTenantModal;
        EmailActivationModal emailActivationModal;
        ForgotPasswordModal forgotPasswordModal;

        public string CurrentTenancyNameOrDefault => _applicationContext.CurrentTenant != null
        ? _applicationContext.CurrentTenant.TenancyName
        : L("NotSelected");


        public Index()
        {
            _accountService = DependencyResolver.Resolve<IAccountService>();
            _accountAppService = DependencyResolver.Resolve<IAccountAppService>();
            _applicationContext = DependencyResolver.Resolve<IApplicationContext>();
        }

        protected override async Task OnInitializedAsync()
        {
            await _accountService.LogoutAsync();
            _accountService.AbpAuthenticateModel.TwoFactorVerificationCode = "";
        }

        private async Task LoginUser()
        {
            await _accountService.LoginUserAsync();
        }

        private async Task SwitchTenantButton()
        {
            await switchTenantModal.Show();
        }

        private async Task EmailActivationButton()
        {
            await emailActivationModal.Show();
        }

        private async Task ForgotPasswordButton()
        {
            await forgotPasswordModal.Show();
        }

        public async Task OnSwitchTenantSave(string tenantName)
        {
            if (string.IsNullOrEmpty(tenantName))
            {
                _applicationContext.SetAsHost();
                ApiUrlConfig.ResetBaseUrl();
            }
            else
            {
                await SetTenantAsync(tenantName);
            }
        }

        private async Task SetTenantAsync(string tenancyName)
        {
            await SetBusyAsync(async () =>
            {
                await WebRequestExecuter.Execute(
                    async () => await _accountAppService.IsTenantAvailable(
                        new IsTenantAvailableInput { TenancyName = tenancyName }),
                    result => IsTenantAvailableExecuted(result, tenancyName)
                );
            });
        }

        private async Task IsTenantAvailableExecuted(IsTenantAvailableOutput result, string tenancyName)
        {
            var tenantAvailableResult = result;

            switch (tenantAvailableResult.State)
            {
                case TenantAvailabilityState.Available:
                    _applicationContext.SetAsTenant(tenancyName, tenantAvailableResult.TenantId.Value);
                    ApiUrlConfig.ChangeBaseUrl(tenantAvailableResult.ServerRootAddress);
                    break;
                case TenantAvailabilityState.InActive:
                    await UserDialogsService.UnBlock();
                    await UserDialogsService.AlertError(L("TenantIsNotActive", tenancyName));
                    break;
                case TenantAvailabilityState.NotFound:
                    await UserDialogsService.UnBlock();
                    await UserDialogsService.AlertError(L("ThereIsNoTenantDefinedWithName{0}", tenancyName));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public async Task OnEmailActivation()
        {
            await emailActivationModal.Hide();
            await UserDialogsService.AlertSuccess(L("SendEmailActivationLink_Information"));
        }

        public async Task OnForgotPassword()
        {
            await forgotPasswordModal.Hide();
            await UserDialogsService.AlertSuccess(L("PasswordResetMailSentMessage"));
        }

        private void OnLanguageSwitchAsync()
        {
            StateHasChanged();
        }
    }
}
