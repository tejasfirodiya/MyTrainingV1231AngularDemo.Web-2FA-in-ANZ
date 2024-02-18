using Abp.AspNetCore.Mvc.Authorization;
using MyTrainingV1231AngularDemo.Authorization;
using MyTrainingV1231AngularDemo.Storage;
using Abp.BackgroundJobs;

namespace MyTrainingV1231AngularDemo.Web.Controllers
{
    [AbpMvcAuthorize(AppPermissions.Pages_Administration_Users)]
    public class UsersController : UsersControllerBase
    {
        public UsersController(IBinaryObjectManager binaryObjectManager, IBackgroundJobManager backgroundJobManager)
            : base(binaryObjectManager, backgroundJobManager)
        {
        }
    }
}