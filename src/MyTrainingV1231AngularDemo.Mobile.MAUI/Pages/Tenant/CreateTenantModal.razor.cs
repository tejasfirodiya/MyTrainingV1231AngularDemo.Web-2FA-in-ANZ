using Abp.ObjectMapping;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MyTrainingV1231AngularDemo.Common;
using MyTrainingV1231AngularDemo.Core.Dependency;
using MyTrainingV1231AngularDemo.Core.Threading;
using MyTrainingV1231AngularDemo.Editions.Dto;
using MyTrainingV1231AngularDemo.Mobile.MAUI.Extensions;
using MyTrainingV1231AngularDemo.Mobile.MAUI.Shared;
using MyTrainingV1231AngularDemo.MultiTenancy;
using MyTrainingV1231AngularDemo.MultiTenancy.Dto;
using MyTrainingV1231AngularDemo.Validations;

namespace MyTrainingV1231AngularDemo.Mobile.MAUI.Pages.Tenant
{
    public partial class CreateTenantModal : ModalBase
    {
        [Parameter] public EventCallback OnSaveCompleted { get; set; }

        public override string ModalId => "create-tenant-modal";

        protected IObjectMapper ObjectMapper { get; set; }

        private ITenantAppService _tenantAppService;
        private readonly ICommonLookupAppService _commonLookupAppService;


        private const string NotAssignedValue = "0";

        private CreateTenantInput TenantToCreate { get; set; }

        private bool _isUnlimitedTimeSubscription = true;

        public bool IsUnlimitedTimeSubscription
        {
            get => _isUnlimitedTimeSubscription;
            set
            {
                _isUnlimitedTimeSubscription = value;
                StateHasChanged();
            }
        }

        public List<SubscribableEditionComboboxItemDto> Editions { get; set; }

        private SubscribableEditionComboboxItemDto _selectedEdition;

        public SubscribableEditionComboboxItemDto SelectedEdition
        {
            get => _selectedEdition;
            set
            {
                _selectedEdition = value;
                if (_isInitialized)
                {
                    UpdateModel();
                }

                IsSubscriptionFieldVisible = SelectedEdition != null && SelectedEdition.Value != NotAssignedValue;
                StateHasChanged();
            }
        }

        private bool _isInitialized;

        public bool IsSelectedEditionFree
        {
            get
            {
                if (TenantToCreate == null)
                {
                    return true;
                }

                if (!TenantToCreate.EditionId.HasValue)
                {
                    return true;
                }

                if (!SelectedEdition.IsFree.HasValue)
                {
                    return true;
                }

                return SelectedEdition.IsFree.Value;
            }
        }

        private bool _isSubscriptionFieldVisible;

        public bool IsSubscriptionFieldVisible
        {
            get => _isSubscriptionFieldVisible;
            set
            {
                _isSubscriptionFieldVisible = value;
            }
        }

        private bool _useHostDatabase = true;
        public bool UseHostDatabase
        {
            get => _useHostDatabase;
            set
            {
                _useHostDatabase = value;
                if (value)
                {
                    TenantToCreate.ConnectionString = String.Empty;
                }
                StateHasChanged();
            }
        }

        private bool _isSetRandomPassword = true;
        public bool IsSetRandomPassword
        {
            get => _isSetRandomPassword;
            set
            {
                _isSetRandomPassword = value;
                if (_isSetRandomPassword)
                {
                    TenantToCreate.AdminPassword = null;
                    AdminPasswordRepeat = null;
                }

                StateHasChanged();
            }
        }

        protected string AdminPasswordRepeat { get; set; }

        public CreateTenantModal()
        {
            ObjectMapper = DependencyResolver.Resolve<IObjectMapper>();
            _tenantAppService = DependencyResolver.Resolve<ITenantAppService>();
            _commonLookupAppService = DependencyResolver.Resolve<ICommonLookupAppService>();
        }

        public async Task Open()
        {
            _isInitialized = false;
            TenantToCreate = new CreateTenantInput()
            {
                IsActive = true,
            };

            await SetBusyAsync(async () =>
            {
                await PopulateEditionsCombobox();
                _isInitialized = true;
                StateHasChanged();
                await Show();
            });
        }

        private async Task PopulateEditionsCombobox()
        {
            var editions = await _commonLookupAppService.GetEditionsForCombobox();
            Editions = editions.Items.ToList();
            AddNotAssignedItem();
        }

        private void AddNotAssignedItem()
        {
            Editions.Insert(0, new SubscribableEditionComboboxItemDto(NotAssignedValue,
                string.Format("- {0} -", L("NotAssigned")), null));
        }

        private void SetSelectedEdition(string editionId)
        {
            SelectedEdition = !string.IsNullOrEmpty(editionId) ?
                Editions.Single(e => e.Value == editionId) :
                Editions.Single(e => e.Value == NotAssignedValue);
        }

        private async Task CreateTenantAsync()
        {
            var isValid = true;

            await SetBusyAsync(async () =>
            {
                await WebRequestExecuter.Execute(async () =>
                {
                    NormalizeCreateTenantInput(TenantToCreate);

                    if (!await ValidateInput(TenantToCreate))
                    {
                        isValid = false;
                        return;

                    }

                    await _tenantAppService.CreateTenant(TenantToCreate);

                }, async () =>
                {
                    if (!isValid)
                    {
                        return;
                    }

                    TenantToCreate = null;
                    _isInitialized = false;

                    await UserDialogsService.AlertSuccess(L("SuccessfullySaved"));
                    StateHasChanged();
                    await Hide();
                    await OnSaveCompleted.InvokeAsync();
                });
            });

        }

        private async Task<bool> ValidateInput(object input)
        {
            if (TenantToCreate.AdminPassword != AdminPasswordRepeat)
            {
                await UserDialogsService.AlertWarn("PasswordsDontMatch");
                return false;
            }

            var validationResult = DataAnnotationsValidator.Validate(input);
            if (validationResult.IsValid)
            {
                return true;
            }

            await UserDialogsService.AlertWarn(validationResult.ConsolidatedMessage);

            return false;
        }

        private void NormalizeCreateTenantInput(CreateTenantInput input)
        {
            input.EditionId = NormalizeEditionId(input.EditionId);
            input.SubscriptionEndDateUtc = NormalizeSubscriptionEndDateUtc(input.SubscriptionEndDateUtc);
        }

        private int? NormalizeEditionId(int? editionId)
        {
            return editionId.HasValue && editionId.Value == 0 ? null : editionId;
        }

        private DateTime? NormalizeSubscriptionEndDateUtc(DateTime? subscriptionEndDateUtc)
        {
            if (IsUnlimitedTimeSubscription)
            {
                return null;
            }

            return subscriptionEndDateUtc.GetEndOfDate();
        }

        public override Task Hide()
        {
            _isInitialized = false;
            TenantToCreate = null;
            StateHasChanged();
            return base.Hide();
        }

        public override async Task Show()
        {
            await base.Show();
        }

        private void UpdateModel()
        {
            if (SelectedEdition != null &&
                int.TryParse(SelectedEdition.Value, out var selectedEditionId))
            {
                TenantToCreate.EditionId = selectedEditionId;
            }
            else
            {
                TenantToCreate.EditionId = null;
            }

            TenantToCreate.IsInTrialPeriod = !IsSelectedEditionFree;
        }

        public void OnSelectedEditionChanged(ChangeEventArgs e)
        {
            var newValue = e.Value.ToString();

            if (int.TryParse(newValue, out int id))
            {
                SetSelectedEdition(newValue);

            }
            else
            {
                SelectedEdition = !string.IsNullOrEmpty(newValue) ?
               Editions.Single(e => e.DisplayText == newValue) :
               Editions.Single(e => e.Value == NotAssignedValue);
            }
        }
    }
}
