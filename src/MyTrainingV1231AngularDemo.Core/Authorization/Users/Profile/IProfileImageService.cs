using Abp;
using Abp.Domain.Services;
using System.Threading.Tasks;

namespace MyTrainingV1231AngularDemo.Authorization.Users.Profile
{
    public interface IProfileImageService : IDomainService
    {
        Task<string> GetProfilePictureContentForUser(UserIdentifier userIdentifier);
    }
}