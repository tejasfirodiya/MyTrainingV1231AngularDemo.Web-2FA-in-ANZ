using System.Threading.Tasks;
using Abp;
using Abp.Dependency;
using MyTrainingV1231AngularDemo.Configuration.Dto;
using MyTrainingV1231AngularDemo.UiCustomization.Dto;

namespace MyTrainingV1231AngularDemo.UiCustomization
{
    public interface IUiCustomizer : ISingletonDependency
    {
        Task<UiCustomizationSettingsDto> GetUiSettings();

        Task UpdateUserUiManagementSettingsAsync(UserIdentifier user, ThemeSettingsDto settings);

        Task UpdateTenantUiManagementSettingsAsync(int tenantId, ThemeSettingsDto settings, UserIdentifier changerUser);

        Task UpdateApplicationUiManagementSettingsAsync(ThemeSettingsDto settings, UserIdentifier changerUser);

        Task<ThemeSettingsDto> GetHostUiManagementSettings();

        Task<ThemeSettingsDto> GetTenantUiCustomizationSettings(int tenantId);

        Task UpdateDarkModeSettingsAsync(UserIdentifier user, bool isDarkModeEnabled);

        Task<string> GetBodyClass();

        Task<string> GetBodyStyle();
    }
}