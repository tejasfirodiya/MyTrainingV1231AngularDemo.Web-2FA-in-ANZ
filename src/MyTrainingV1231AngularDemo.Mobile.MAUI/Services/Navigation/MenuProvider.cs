using Abp.Dependency;
using Abp.Extensions;
using MyTrainingV1231AngularDemo.Authorization;
using MyTrainingV1231AngularDemo.Localization;
using MyTrainingV1231AngularDemo.Models.NavigationMenu;
using MyTrainingV1231AngularDemo.Services.Permission;

namespace MyTrainingV1231AngularDemo.Services.Navigation
{
    public class MenuProvider : ISingletonDependency, IMenuProvider
    {
        /* For more icons:
            https://material.io/icons/
        */
        private List<NavigationMenuItem> _menuItems;

        public void InitializeMenuItems()
        {
            _menuItems = new List<NavigationMenuItem>
            {
                new NavigationMenuItem
                {
                    Title = L.Localize("Tenants"),
                    Icon = "fa-solid fa-list",
                    NavigationUrl = NavigationUrlConsts.Tenants,
                    RequiredPermissionName = AppPermissions.Pages_Tenants,
                },
                new NavigationMenuItem
                {
                    Title = L.Localize("Users"),
                    Icon = "fa-solid fa-filter",
                    NavigationUrl= NavigationUrlConsts.User,
                    RequiredPermissionName = AppPermissions.Pages_Administration_Users,
                },
                new NavigationMenuItem
                {
                    Title = L.Localize("MySettings"),
                    Icon = "fa-solid fa-cog",
                    NavigationUrl  = NavigationUrlConsts.Settings
                }

                /*This is a sample menu item to guide how to add a new item.
                    ,new NavigationMenuItem
                    {
                        Title = "Sample View",
                        Icon = "MyIcon.png",
                        TargetType = typeof(_SampleView),
                        Order = 10
                    }
                */
            };
        }

        public List<NavigationMenuItem> GetAuthorizedMenuItems(Dictionary<string, string> grantedPermissions)
        {
            InitializeMenuItems();
            return FilterAuthorizedMenuItems(_menuItems, grantedPermissions);
        }

        private List<NavigationMenuItem> FilterAuthorizedMenuItems(List<NavigationMenuItem> menuItems, Dictionary<string, string> grantedPermissions)
        {
            var authorizedMenuItems = new List<NavigationMenuItem>();
            foreach (var menuItem in menuItems)
            {
                var authorizedMenuItem = new NavigationMenuItem()
                {
                    Title = menuItem.Title,
                    Icon = menuItem.Icon,
                    IsSelected = menuItem.IsSelected,
                    NavigationParameter = menuItem.NavigationParameter,
                    NavigationUrl = menuItem.NavigationUrl,
                    RequiredPermissionName = menuItem.RequiredPermissionName
                };

                if (menuItem.Items.Any())
                {
                    var authorizedSubMenuItems = FilterAuthorizedMenuItems(menuItem.Items, grantedPermissions);
                    if (authorizedMenuItem.NavigationUrl.IsNullOrEmpty() && !authorizedSubMenuItems.Any())
                    {
                        continue;
                    }

                    authorizedMenuItem.Items.AddRange(authorizedSubMenuItems);
                }

                if (menuItem.RequiredPermissionName == null)
                {
                    authorizedMenuItems.Add(authorizedMenuItem);
                    continue;
                }

                if (grantedPermissions != null &&
                    grantedPermissions.ContainsKey(menuItem.RequiredPermissionName))
                {
                    authorizedMenuItems.Add(authorizedMenuItem);
                }
            }

            return authorizedMenuItems;
        }
    }
}