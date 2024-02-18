using Abp.Authorization;
using MyTrainingV1231AngularDemo.Authorization.Roles;
using MyTrainingV1231AngularDemo.Authorization.Users;

namespace MyTrainingV1231AngularDemo.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}
