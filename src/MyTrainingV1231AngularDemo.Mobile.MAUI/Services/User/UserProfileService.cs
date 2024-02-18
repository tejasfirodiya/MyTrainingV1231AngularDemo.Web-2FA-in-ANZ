using Abp.Dependency;
using MyTrainingV1231AngularDemo.Authorization.Users.Profile;

namespace MyTrainingV1231AngularDemo.Mobile.MAUI.Services.User
{
    public class UserProfileService : IUserProfileService, ITransientDependency
    {
        private readonly IProfileAppService _profileAppService;

        public UserProfileService(IProfileAppService profileAppService)
        {
            _profileAppService = profileAppService;
        }

        public async Task<string> GetProfilePicture(long userId)
        {
            var result = await _profileAppService.GetProfilePictureByUser(userId);
            if (string.IsNullOrWhiteSpace(result.ProfilePicture))
            {
                return GetDefaultProfilePicture();
            }

            return "data:image/png;base64, " + result.ProfilePicture;
        }

        public string GetDefaultProfilePicture()
        {
            return "media/default-profile-picture.png";
        }
    }
}
