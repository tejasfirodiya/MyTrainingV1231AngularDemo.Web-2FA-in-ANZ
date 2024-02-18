using MyTrainingV1231AngularDemo.Models.NavigationMenu;

namespace MyTrainingV1231AngularDemo.Services.Navigation
{
    public interface IMenuProvider
    {
        List<NavigationMenuItem> GetAuthorizedMenuItems(Dictionary<string, string> grantedPermissions);
    }
}