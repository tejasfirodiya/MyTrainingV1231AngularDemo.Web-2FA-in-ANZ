using Abp.AspNetCore.Mvc.Authorization;
using MyTrainingV1231AngularDemo.Authorization.Users.Profile;
using MyTrainingV1231AngularDemo.Graphics;
using MyTrainingV1231AngularDemo.Storage;

namespace MyTrainingV1231AngularDemo.Web.Controllers
{
    [AbpMvcAuthorize]
    public class ProfileController : ProfileControllerBase
    {
        public ProfileController(
            ITempFileCacheManager tempFileCacheManager,
            IProfileAppService profileAppService,
            IImageValidator imageValidator) :
            base(tempFileCacheManager, profileAppService, imageValidator)
        {
        }
    }
}