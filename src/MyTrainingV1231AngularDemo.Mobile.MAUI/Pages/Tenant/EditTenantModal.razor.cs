using Abp.ObjectMapping;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MyTrainingV1231AngularDemo.Common;
using MyTrainingV1231AngularDemo.Core.Dependency;
using MyTrainingV1231AngularDemo.Core.Threading;
using MyTrainingV1231AngularDemo.Editions.Dto;
using MyTrainingV1231AngularDemo.Mobile.MAUI.Extensions;
using MyTrainingV1231AngularDemo.Mobile.MAUI.Models.Tenants;
using MyTrainingV1231AngularDemo.Mobile.MAUI.Shared;
using MyTrainingV1231AngularDemo.MultiTenancy;
using MyTrainingV1231AngularDemo.MultiTenancy.Dto;
using MyTrainingV1231AngularDemo.Validations;

namespace MyTrainingV1231AngularDemo.Mobile.MAUI.Pages.Tenant
{
    public partial class EditTenantModal : ModalBase
    {
        [Parameter] public EventCallback OnSaveCompleted { get; set; }

        public override string ModalId => "edit-tenant-modal";

        protected IObjectMapper ObjectMapper { get; set; }

        private ITenantAppService _tenantAppService;

        private readonly ICommonLookupAppService _commonLookupAppService;

        private TenantListDto TenantToEdit { get; set; }

        private const string NotAssignedValue = "0";

        private bool _isUnlimitedTimeSubscription;
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
                if (TenantToEdit == null)
                {
                    return true;
                }

                if (!TenantToEdit.EditionId.HasValue)
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

        public EditTenantModal()
        {
            ObjectMapper = DependencyResolver.Resolve<IObjectMapper>();
            _tenantAppService = DependencyResolver.Resolve<ITenantAppService>();
            _commonLookupAppService = DependencyResolver.Resolve<ICommonLookupAppService>();
        }

        public async Task OpenFor(TenantListDto tenantToEdit)
        {
            _isInitialized = false;
            TenantToEdit = tenantToEdit;
            IsUnlimitedTimeSubscription = TenantToEdit?.SubscriptionEndDateUtc == null;

            StateHasChanged();

            await SetBusyAsync(async () =>
            {
                await PopulateEditionsCombobox();
                SetSelectedEdition(tenantToEdit.EditionId);
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

        private void SetSelectedEdition(int? editionId)
        {
            SetSelectedEdition(editionId.HasValue ? editionId.Value.ToString() : null);
        }

        private void SetSelectedEdition(string editionId)
        {
            SelectedEdition = !string.IsNullOrEmpty(editionId) ?
                Editions.Single(e => e.Value == editionId) :
                Editions.Single(e => e.Value == NotAssignedValue);
        }

        private async Task UpdateTenantAsync()
        {
            var isValid = true;

            await SetBusyAsync(async () =>
            {
                await WebRequestExecuter.Execute(async () =>
                {
                    var model = ObjectMapper.Map<TenantListModel>(TenantToEdit);
                    var input = ObjectMapper.Map<TenantEditDto>(model);
                    NormalizeTenantUpdateInput(input);

                    if (!await ValidateInput(input))
                    {
                        isValid = false;
                        return;

                    }

                    await _tenantAppService.UpdateTenant(input);

                }, async () =>
                {
                    if (!isValid)
                    {
                        return;
                    }

                    TenantToEdit = null;
                    await UserDialogsService.AlertSuccess(L("SuccessfullySaved"));
                    StateHasChanged();
                    await Hide();
                    await OnSaveCompleted.InvokeAsync();
                });
            });

        }

        private async Task<bool> ValidateInput(object input)
        {
            var validationResult = DataAnnotationsValidator.Validate(input);
            if (validationResult.IsValid)
            {
                return true;
            }

            await UserDialogsService.AlertWarn(validationResult.ConsolidatedMessage);

            return false;
        }

        private void NormalizeTenantUpdateInput(TenantEditDto input)
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
            TenantToEdit = null;
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
                TenantToEdit.EditionId = selectedEditionId;
            }
            else
            {
                TenantToEdit.EditionId = null;
            }

            TenantToEdit.IsInTrialPeriod = !IsSelectedEditionFree;
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
