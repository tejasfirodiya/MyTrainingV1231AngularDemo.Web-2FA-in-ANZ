using System.Threading.Tasks;
using Abp;
using Abp.Configuration;
using MyTrainingV1231AngularDemo.Configuration;
using MyTrainingV1231AngularDemo.Configuration.Dto;
using MyTrainingV1231AngularDemo.UiCustomization;
using MyTrainingV1231AngularDemo.UiCustomization.Dto;

namespace MyTrainingV1231AngularDemo.Web.UiCustomization.Metronic
{
    public class Theme3UiCustomizer : UiThemeCustomizerBase, IUiCustomizer
    {
        public Theme3UiCustomizer(ISettingManager settingManager)
            : base(settingManager, AppConsts.Theme3)
        {
        }

        public async Task<UiCustomizationSettingsDto> GetUiSettings()
        {
            var settings = new UiCustomizationSettingsDto
            {
                BaseSettings = new ThemeSettingsDto
                {
                    Layout = new ThemeLayoutSettingsDto
                    {
                        DarkMode = await GetSettingValueAsync<bool>(AppSettings.UiManagement.DarkMode)
                    },
                    Footer = new ThemeFooterSettingsDto
                    {
                        FixedFooter = true
                    },
                    Menu = new ThemeMenuSettingsDto
                    {
                        SearchActive = await GetSettingValueAsync<bool>(AppSettings.UiManagement.SearchActive)
                    }
                }
            };

            settings.BaseSettings.Theme = ThemeName;

            settings.BaseSettings.Layout.LayoutType = "fluid";

            settings.BaseSettings.Menu.FixedAside = true;
            settings.BaseSettings.Menu.Position = "left";
            settings.BaseSettings.Menu.AsideSkin = "dark";
            settings.BaseSettings.Menu.SubmenuToggle = "false";
            settings.BaseSettings.Menu.EnableSecondary = true;

            settings.BaseSettings.SubHeader.SubheaderSize = 1;
            settings.BaseSettings.SubHeader.TitleStyle = "subheader-title text-dark fw-bold my-2 me-3";
            settings.BaseSettings.SubHeader.ContainerStyle = "subheader py-3 py-lg-8  subheader-transparent ";

            settings.IsLeftMenuUsed = true;
            settings.IsTopMenuUsed = false;
            settings.IsTabMenuUsed = false;

            return settings;
        }

        public async Task UpdateUserUiManagementSettingsAsync(UserIdentifier user, ThemeSettingsDto settings)
        {
            await SettingManager.ChangeSettingForUserAsync(user, AppSettings.UiManagement.Theme, ThemeName);

            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.DarkMode,
                settings.Layout.DarkMode.ToString());
            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.Header.DesktopFixedHeader,
                settings.Header.DesktopFixedHeader.ToString());
            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.Header.MobileFixedHeader,
                settings.Header.MobileFixedHeader.ToString());
            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.SubHeader.Fixed,
                settings.SubHeader.FixedSubHeader.ToString());
            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.SubHeader.Style,
                settings.SubHeader.SubheaderStyle);
            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.Footer.FixedFooter,
                settings.Footer.FixedFooter.ToString());
            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.SearchActive,
                settings.Menu.SearchActive.ToString());
        }

        public async Task UpdateTenantUiManagementSettingsAsync(int tenantId, ThemeSettingsDto settings, UserIdentifier changerUser)
        {
            await SettingManager.ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.Theme, ThemeName);

            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.DarkMode,
                settings.Layout.DarkMode.ToString());
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.Header.DesktopFixedHeader,
                settings.Header.DesktopFixedHeader.ToString());
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.Header.MobileFixedHeader,
                settings.Header.MobileFixedHeader.ToString());
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.SubHeader.Fixed,
                settings.SubHeader.FixedSubHeader.ToString());
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.SubHeader.Style,
                settings.SubHeader.SubheaderStyle);
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.Footer.FixedFooter,
                settings.Footer.FixedFooter.ToString());
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.SearchActive,
                settings.Menu.SearchActive.ToString());
            
            await ResetDarkModeSettingsAsync(changerUser);
        }

        public async Task UpdateApplicationUiManagementSettingsAsync(ThemeSettingsDto settings, UserIdentifier changerUser)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.UiManagement.Theme, ThemeName);

            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.DarkMode,
                settings.Layout.DarkMode.ToString());
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.Header.DesktopFixedHeader,
                settings.Header.DesktopFixedHeader.ToString());
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.Header.MobileFixedHeader,
                settings.Header.MobileFixedHeader.ToString());
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.SubHeader.Fixed,
                settings.SubHeader.FixedSubHeader.ToString());
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.SubHeader.Style,
                settings.SubHeader.SubheaderStyle);
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.Footer.FixedFooter,
                settings.Footer.FixedFooter.ToString());
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.SearchActive,
                settings.Menu.SearchActive.ToString());
            
            await ResetDarkModeSettingsAsync(changerUser);
        }

        public async Task<ThemeSettingsDto> GetHostUiManagementSettings()
        {
            var theme = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.UiManagement.Theme);

            return new ThemeSettingsDto
            {
                Theme = theme,
                Layout = new ThemeLayoutSettingsDto()
                {
                    LayoutType = "fluid",
                    DarkMode = await GetSettingValueForApplicationAsync<bool>(AppSettings.UiManagement.DarkMode),
                },
                SubHeader = new ThemeSubHeaderSettingsDto()
                {
                    FixedSubHeader =
                        await GetSettingValueForApplicationAsync<bool>(AppSettings.UiManagement.SubHeader.Fixed),
                    SubheaderStyle = await GetSettingValueForApplicationAsync(AppSettings.UiManagement.SubHeader.Style)
                },
                Footer = new ThemeFooterSettingsDto
                {
                    FixedFooter =
                        await GetSettingValueForApplicationAsync<bool>(AppSettings.UiManagement.Footer.FixedFooter)
                },
                Menu = new ThemeMenuSettingsDto()
                {
                    SearchActive = await GetSettingValueForApplicationAsync<bool>(AppSettings.UiManagement.SearchActive)
                }
            };
        }

        public async Task<ThemeSettingsDto> GetTenantUiCustomizationSettings(int tenantId)
        {
            var theme = await SettingManager.GetSettingValueForTenantAsync(AppSettings.UiManagement.Theme, tenantId);

            return new ThemeSettingsDto
            {
                Theme = theme,
                Layout = new ThemeLayoutSettingsDto
                {
                    LayoutType = "fluid",
                    DarkMode = await GetSettingValueForTenantAsync<bool>(AppSettings.UiManagement.DarkMode, tenantId)
                },
                SubHeader = new ThemeSubHeaderSettingsDto
                {
                    FixedSubHeader =
                        await GetSettingValueForTenantAsync<bool>(AppSettings.UiManagement.SubHeader.Fixed, tenantId),
                    SubheaderStyle =
                        await GetSettingValueForTenantAsync(AppSettings.UiManagement.SubHeader.Style, tenantId)
                },
                Footer = new ThemeFooterSettingsDto
                {
                    FixedFooter =
                        await GetSettingValueForTenantAsync<bool>(AppSettings.UiManagement.Footer.FixedFooter, tenantId)
                },
                Menu = new ThemeMenuSettingsDto()
                {
                    SearchActive =
                        await GetSettingValueForTenantAsync<bool>(AppSettings.UiManagement.SearchActive, tenantId)
                }
            };
        }
    }
}
