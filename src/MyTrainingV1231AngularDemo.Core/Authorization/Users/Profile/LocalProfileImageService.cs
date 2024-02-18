using System;
using System.Threading.Tasks;
using Abp;
using Abp.Dependency;
using MyTrainingV1231AngularDemo.Storage;

namespace MyTrainingV1231AngularDemo.Authorization.Users.Profile
{
    public class LocalProfileImageService : IProfileImageService, ITransientDependency
    {
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly UserManager _userManager;

        public LocalProfileImageService(
            IBinaryObjectManager binaryObjectManager,
            UserManager userManager)
        {
            _binaryObjectManager = binaryObjectManager;
            _userManager = userManager;
        }

        public async Task<string> GetProfilePictureContentForUser(UserIdentifier userIdentifier)
        {
            var user = await _userManager.GetUserOrNullAsync(userIdentifier);
            if (user?.ProfilePictureId == null)
            {
                return "";
            }

            var file = await _binaryObjectManager.GetOrNullAsync(user.ProfilePictureId.Value);
            return file == null ? "" : Convert.ToBase64String(file.Bytes);
        }
    }
}