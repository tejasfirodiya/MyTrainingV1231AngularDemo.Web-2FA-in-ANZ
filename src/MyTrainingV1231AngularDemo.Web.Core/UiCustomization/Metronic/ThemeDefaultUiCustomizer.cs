using System.Threading.Tasks;
using Abp;
using Abp.Configuration;
using Abp.Localization;
using Abp.UI;
using MyTrainingV1231AngularDemo.Configuration;
using MyTrainingV1231AngularDemo.Configuration.Dto;
using MyTrainingV1231AngularDemo.UiCustomization;
using MyTrainingV1231AngularDemo.UiCustomization.Dto;

namespace MyTrainingV1231AngularDemo.Web.UiCustomization.Metronic
{
    public class ThemeDefaultUiCustomizer : UiThemeCustomizerBase, IUiCustomizer
    {
        private readonly ILocalizationManager _localizationManager;

        public ThemeDefaultUiCustomizer(
            ISettingManager settingManager,
            ILocalizationManager localizationManager)
            : base(settingManager, AppConsts.ThemeDefault)
        {
            _localizationManager = localizationManager;
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
                    SubHeader = new ThemeSubHeaderSettingsDto
                    {
                        FixedSubHeader = true,
                        SubheaderStyle = await GetSettingValueAsync(AppSettings.UiManagement.SubHeader.Style),
                        ContainerStyle = "app-toolbar py-3 py-lg-6"
                    },
                    Menu = new ThemeMenuSettingsDto
                    {
                        AsideSkin = await GetSettingValueAsync(AppSettings.UiManagement.LeftAside.AsideSkin),
                        FixedAside = true,
                        AllowAsideMinimizing =
                            await GetSettingValueAsync<bool>(AppSettings.UiManagement.LeftAside.AllowAsideMinimizing),
                        DefaultMinimizedAside =
                            await GetSettingValueAsync<bool>(AppSettings.UiManagement.LeftAside.DefaultMinimizedAside),
                        SubmenuToggle = await GetSettingValueAsync(AppSettings.UiManagement.LeftAside.SubmenuToggle),
                        HoverableAside =
                            await GetSettingValueAsync<bool>(AppSettings.UiManagement.LeftAside.HoverableAside),
                        SearchActive = await GetSettingValueAsync<bool>(AppSettings.UiManagement.SearchActive),
                    },
                    Footer = new ThemeFooterSettingsDto
                    {
                        FixedFooter = await GetSettingValueAsync<bool>(AppSettings.UiManagement.Footer.FixedFooter)
                    },
                    Toolbar = new ThemeToolbarSettingsDto
                    {
                        DesktopFixedToolbar = await GetSettingValueAsync<bool>(AppSettings.UiManagement.Toolbar.DesktopFixedToolbar),
                        MobileFixedToolbar = await GetSettingValueAsync<bool>(AppSettings.UiManagement.Toolbar.MobileFixedToolbar)
                    }
                }
            };

            settings.BaseSettings.Theme = ThemeName;
            settings.BaseSettings.Layout.LayoutType = "fluid";
            settings.BaseSettings.Menu.Position = "left";
            settings.BaseSettings.SubHeader.SubheaderSize = 1;
            settings.BaseSettings.SubHeader.TitleStyle =
                "page-heading d-flex text-dark fw-bold fs-3 flex-column justify-content-center my-0";
            settings.BaseSettings.SubHeader.ContainerStyle = "app-toolbar py-3 py-lg-6";

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

            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.LeftAside.AsideSkin,
                settings.Menu.AsideSkin);
            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.LeftAside.AllowAsideMinimizing,
                settings.Menu.AllowAsideMinimizing.ToString());
            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.LeftAside.DefaultMinimizedAside,
                settings.Menu.DefaultMinimizedAside.ToString());
            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.LeftAside.SubmenuToggle,
                settings.Menu.SubmenuToggle);
            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.LeftAside.HoverableAside,
                settings.Menu.HoverableAside.ToString());
            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.SearchActive,
                settings.Menu.SearchActive.ToString());

            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.Footer.FixedFooter,
                settings.Footer.FixedFooter.ToString());

            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.Toolbar.DesktopFixedToolbar,
                settings.Toolbar.DesktopFixedToolbar.ToString()
            );
            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.Toolbar.MobileFixedToolbar,
                settings.Toolbar.MobileFixedToolbar.ToString()
            );
        }

        public async Task UpdateTenantUiManagementSettingsAsync(int tenantId, ThemeSettingsDto settings,
            UserIdentifier changerUser)
        {
            await SettingManager.ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.Theme, settings.Theme);

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

            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.LeftAside.AsideSkin,
                settings.Menu.AsideSkin);
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.LeftAside.AllowAsideMinimizing,
                settings.Menu.AllowAsideMinimizing.ToString());
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.LeftAside.DefaultMinimizedAside,
                settings.Menu.DefaultMinimizedAside.ToString());
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.LeftAside.SubmenuToggle,
                settings.Menu.SubmenuToggle);
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.LeftAside.HoverableAside,
                settings.Menu.HoverableAside.ToString());
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.SearchActive,
                settings.Menu.SearchActive.ToString());

            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.Footer.FixedFooter,
                settings.Footer.FixedFooter.ToString());
            
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.Toolbar.DesktopFixedToolbar,
                settings.Toolbar.DesktopFixedToolbar.ToString());
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.Toolbar.MobileFixedToolbar,
                settings.Toolbar.MobileFixedToolbar.ToString());

            await ResetDarkModeSettingsAsync(changerUser);
        }

        public async Task UpdateApplicationUiManagementSettingsAsync(ThemeSettingsDto settings,
            UserIdentifier changerUser)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.UiManagement.Theme, settings.Theme);

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

            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.LeftAside.AsideSkin,
                settings.Menu.AsideSkin);
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.LeftAside.AllowAsideMinimizing,
                settings.Menu.AllowAsideMinimizing.ToString());
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.LeftAside.DefaultMinimizedAside,
                settings.Menu.DefaultMinimizedAside.ToString());
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.LeftAside.SubmenuToggle,
                settings.Menu.SubmenuToggle);
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.LeftAside.HoverableAside,
                settings.Menu.HoverableAside.ToString());
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.SearchActive,
                settings.Menu.SearchActive.ToString());

            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.Footer.FixedFooter,
                settings.Footer.FixedFooter.ToString());
            
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.Toolbar.DesktopFixedToolbar,
                settings.Toolbar.DesktopFixedToolbar.ToString());
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.Toolbar.MobileFixedToolbar,
                settings.Toolbar.MobileFixedToolbar.ToString());

            await ResetDarkModeSettingsAsync(changerUser);
        }

        public async Task<ThemeSettingsDto> GetHostUiManagementSettings()
        {
            var theme = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.UiManagement.Theme);

            return new ThemeSettingsDto
            {
                Theme = theme,
                Layout = new ThemeLayoutSettingsDto
                {
                    DarkMode = await GetSettingValueForApplicationAsync<bool>(AppSettings.UiManagement.DarkMode)
                },
                SubHeader = new ThemeSubHeaderSettingsDto
                {
                    FixedSubHeader =
                        await GetSettingValueForApplicationAsync<bool>(AppSettings.UiManagement.SubHeader.Fixed),
                    SubheaderStyle = await GetSettingValueForApplicationAsync(AppSettings.UiManagement.SubHeader.Style)
                },
                Menu = new ThemeMenuSettingsDto
                {
                    AsideSkin = await GetSettingValueForApplicationAsync(AppSettings.UiManagement.LeftAside.AsideSkin),
                    FixedAside = true,
                    AllowAsideMinimizing =
                        await GetSettingValueForApplicationAsync<bool>(AppSettings.UiManagement.LeftAside
                            .AllowAsideMinimizing),
                    DefaultMinimizedAside =
                        await GetSettingValueForApplicationAsync<bool>(AppSettings.UiManagement.LeftAside
                            .DefaultMinimizedAside),
                    SubmenuToggle =
                        await GetSettingValueForApplicationAsync(AppSettings.UiManagement.LeftAside.SubmenuToggle),
                    HoverableAside =
                        await GetSettingValueForApplicationAsync<bool>(
                            AppSettings.UiManagement.LeftAside.HoverableAside),
                    SearchActive =
                        await GetSettingValueForApplicationAsync<bool>(AppSettings.UiManagement.SearchActive),
                },
                Footer = new ThemeFooterSettingsDto
                {
                    FixedFooter =
                        await GetSettingValueForApplicationAsync<bool>(AppSettings.UiManagement.Footer.FixedFooter)
                },
                Toolbar = new ThemeToolbarSettingsDto
                {
                    DesktopFixedToolbar = await GetSettingValueForApplicationAsync<bool>(AppSettings.UiManagement.Toolbar.DesktopFixedToolbar),
                    MobileFixedToolbar = await GetSettingValueForApplicationAsync<bool>(AppSettings.UiManagement.Toolbar.MobileFixedToolbar)
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
                    DarkMode = await GetSettingValueForTenantAsync<bool>(AppSettings.UiManagement.DarkMode, tenantId),
                },
                SubHeader = new ThemeSubHeaderSettingsDto
                {
                    FixedSubHeader =
                        await GetSettingValueForTenantAsync<bool>(AppSettings.UiManagement.SubHeader.Fixed, tenantId),
                    SubheaderStyle =
                        await GetSettingValueForTenantAsync(AppSettings.UiManagement.SubHeader.Style, tenantId)
                },
                Menu = new ThemeMenuSettingsDto
                {
                    AsideSkin = await GetSettingValueForTenantAsync(AppSettings.UiManagement.LeftAside.AsideSkin,
                        tenantId),
                    FixedAside = true,
                    AllowAsideMinimizing =
                        await GetSettingValueForTenantAsync<bool>(
                            AppSettings.UiManagement.LeftAside.AllowAsideMinimizing, tenantId),
                    DefaultMinimizedAside =
                        await GetSettingValueForTenantAsync<bool>(
                            AppSettings.UiManagement.LeftAside.DefaultMinimizedAside, tenantId),
                    SubmenuToggle =
                        await GetSettingValueForTenantAsync(AppSettings.UiManagement.LeftAside.SubmenuToggle, tenantId),
                    HoverableAside =
                        await GetSettingValueForTenantAsync<bool>(AppSettings.UiManagement.LeftAside.HoverableAside,
                            tenantId),
                    SearchActive = await GetSettingValueForApplicationAsync<bool>(AppSettings.UiManagement.SearchActive)
                },
                Footer = new ThemeFooterSettingsDto
                {
                    FixedFooter =
                        await GetSettingValueForTenantAsync<bool>(AppSettings.UiManagement.Footer.FixedFooter, tenantId)
                },
                Toolbar = new ThemeToolbarSettingsDto
                {
                    DesktopFixedToolbar = await GetSettingValueForTenantAsync<bool>(AppSettings.UiManagement.Toolbar.DesktopFixedToolbar, tenantId),
                    MobileFixedToolbar = await GetSettingValueForTenantAsync<bool>(AppSettings.UiManagement.Toolbar.MobileFixedToolbar, tenantId)
                }
            };
        }

        protected override async Task ResetDarkModeSettingsAsync(UserIdentifier user)
        {
            await base.ResetDarkModeSettingsAsync(user);

            string asideSkinSetting;
            if (user.TenantId.HasValue)
            {
                asideSkinSetting = await GetSettingValueForTenantAsync(AppSettings.UiManagement.LeftAside.AsideSkin,
                    user.TenantId.Value);
            }
            else
            {
                asideSkinSetting =
                    await GetSettingValueForApplicationAsync(AppSettings.UiManagement.LeftAside.AsideSkin);
            }

            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.LeftAside.AsideSkin, asideSkinSetting);
        }
    }
}