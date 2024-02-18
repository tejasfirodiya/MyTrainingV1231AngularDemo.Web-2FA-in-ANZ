using Abp.Application.Services.Dto;
using Microsoft.AspNetCore.Components;
using MyTrainingV1231AngularDemo.Authorization;
using MyTrainingV1231AngularDemo.Authorization.Users;
using MyTrainingV1231AngularDemo.Authorization.Users.Dto;
using MyTrainingV1231AngularDemo.Core.Dependency;
using MyTrainingV1231AngularDemo.Core.Threading;
using MyTrainingV1231AngularDemo.Mobile.MAUI.Models.User;
using MyTrainingV1231AngularDemo.Mobile.MAUI.Services.User;
using MyTrainingV1231AngularDemo.Mobile.MAUI.Shared;
using MyTrainingV1231AngularDemo.Services.Permission;
using MyTrainingV1231AngularDemo.Validations;

namespace MyTrainingV1231AngularDemo.Mobile.MAUI.Pages.User
{
    public partial class CreateOrEditUserModal : ModalBase
    {
        public override string ModalId => "create-or-edit-user";

        [Parameter] public EventCallback OnSave { get; set; }

        protected IPermissionService PermissionService;
        protected IUserAppService UserAppService;
        protected IUserProfileService UserProfileService;

        protected UserCreateOrUpdateModel UserInput;
        protected UserEditOrCreateModel Model;

        private bool _isDeleteButtonVisible;
        private bool _isUnlockButtonVisible;
        private string _pageTitle;

        private bool _isNewUser;
        public bool IsNewUser
        {
            get => _isNewUser;
            set
            {
                _isNewUser = value;
                _isDeleteButtonVisible = !_isNewUser && PermissionService.HasPermission(AppPermissions.Pages_Administration_Users_Delete);
                _isUnlockButtonVisible = !_isNewUser && PermissionService.HasPermission(AppPermissions.Pages_Administration_Users_Edit);
                _pageTitle = _isNewUser ? L("CreatingNewUser") : L("EditUser");

                SetRandomPassword = _isNewUser;
                StateHasChanged();
            }
        }

        private bool _setRandomPassword;
        public bool SetRandomPassword
        {
            get => _setRandomPassword;
            set
            {
                _setRandomPassword = value;
                UserInput.SetRandomPassword = value;
                if (Model != null && Model.User != null)
                {
                    Model.User.Password = "";
                }
                _newPasswordRepeat = "";
            }
        }

        private string _newPasswordRepeat;

        private bool _isInitialized;

        public CreateOrEditUserModal()
        {
            PermissionService = DependencyResolver.Resolve<IPermissionService>();
            UserAppService = DependencyResolver.Resolve<IUserAppService>();
            UserProfileService = DependencyResolver.Resolve<IUserProfileService>();
        }

        public async Task OpenFor(UserListModel userListModel)
        {
            _isInitialized = false;
            await SetBusyAsync(async () =>
            {
                UserInput = new UserCreateOrUpdateModel();
                IsNewUser = userListModel == null;
                UserInput.SendActivationEmail = IsNewUser;

                await WebRequestExecuter.Execute(
                   async () => await UserAppService.GetUserForEdit(new NullableIdDto<long>(userListModel?.Id)),
                   async (user) =>
                   {
                       Model = ObjectMapper.Map<UserEditOrCreateModel>(user);
                       Model.OrganizationUnits = ObjectMapper.Map<List<OrganizationUnitModel>>(user.AllOrganizationUnits);

                       if (IsNewUser)
                       {
                           Model.Photo = UserProfileService.GetDefaultProfilePicture();
                           Model.User = new UserEditDto
                           {
                               IsActive = true,
                               IsLockoutEnabled = true,
                               ShouldChangePasswordOnNextLogin = true,
                           };
                       }
                       else
                       {
                           Model.Photo = userListModel.Photo;
                           Model.CreationTime = userListModel.CreationTime;
                           Model.IsEmailConfirmed = userListModel.IsEmailConfirmed;
                       }

                       _isInitialized = true;
                       await Show();
                   }
                );
            });
        }

        private async Task<bool> ValidateInput()
        {
            // Since DataAnnotationsValidator doesn't work for nested object validation.
            // We manually do validation for each nested object.
            var userInputValidationResult = DataAnnotationsValidator.Validate(UserInput);
            var userValidationResult = DataAnnotationsValidator.Validate(UserInput.User);

            if (userInputValidationResult.IsValid && userValidationResult.IsValid)
            {
                return true;
            }

            await UserDialogsService.AlertWarn(userInputValidationResult.AddRange(userValidationResult.ValidationErrors).ConsolidatedMessage);
            return false;
        }

        private async Task SaveUser()
        {
            UserInput.User = Model.User;
            UserInput.AssignedRoleNames = Model.Roles.Where(x => x.IsAssigned).Select(x => x.RoleName).ToArray();
            UserInput.OrganizationUnits = Model.OrganizationUnits.Where(x => x.IsAssigned).Select(x => x.Id).ToList();

            if (!SetRandomPassword && (!string.IsNullOrWhiteSpace(UserInput.User.Password) || !string.IsNullOrWhiteSpace(_newPasswordRepeat)))
            {
                if (UserInput.User.Password != _newPasswordRepeat)
                {
                    await UserDialogsService.AlertWarn(L("PasswordsDontMatch"));
                }
            }

            if (!await ValidateInput())
            {
                return;
            }

            await SetBusyAsync(async () =>
            {
                await WebRequestExecuter.Execute(
                  async () => await UserAppService.CreateOrUpdateUser(UserInput),
                  async () =>
                  {
                      await UserDialogsService.AlertSuccess(L("SuccessfullySaved"));
                      await Hide();
                      await OnSave.InvokeAsync();
                  }
               );
            });
        }

        private string OUCodeIntentConverter(string code)
        {
            var resultWithIndent = "";

            var indentCharacter = ".";
            foreach (var character in code)
            {
                if (character == '.')
                {
                    resultWithIndent += indentCharacter;
                }
            }

            return resultWithIndent;
        }

        private async Task UnlockUser()
        {
            if (Model.User == null || !Model.User.Id.HasValue)
            {
                return;
            }

            await SetBusyAsync(async () =>
            {
                await WebRequestExecuter.Execute(
                    async () => await UserAppService.UnlockUser(new EntityDto<long>(Model.User.Id.Value)),
                    async () =>
                    {
                        await UserDialogsService.AlertSuccess(L("UnlockedTheUser", Model.User.UserName));
                        await Hide();
                    }
                );
            });
        }

        public async Task DeleteUser()
        {
            if (Model.User == null || !Model.User.Id.HasValue)
            {
                return;
            }

            var isConfirmed = await UserDialogsService.Confirm(L("UserDeleteWarningMessage", "\"" + Model.User.UserName + "\""), L("AreYouSure"));
            if (isConfirmed)
            {
                await SetBusyAsync(async () =>
                {
                    await WebRequestExecuter.Execute(
                       async () => await UserAppService.DeleteUser(new EntityDto<long>(Model.User.Id.Value)),
                       async () =>
                       {
                           await UserDialogsService.AlertSuccess(L("SuccessfullyDeleted"));
                           await Hide();
                           await OnSave.InvokeAsync();
                       }
                    );
                });
            }
        }
    }
}
