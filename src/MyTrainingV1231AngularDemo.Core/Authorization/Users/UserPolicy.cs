using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.UI;
using MyTrainingV1231AngularDemo.Features;

namespace MyTrainingV1231AngularDemo.Authorization.Users
{
    public class UserPolicy : MyTrainingV1231AngularDemoServiceBase, IUserPolicy
    {
        private readonly IFeatureChecker _featureChecker;
        private readonly IRepository<User, long> _userRepository;

        public UserPolicy(IFeatureChecker featureChecker,IRepository<User, long> userRepository)
        {
            _featureChecker = featureChecker;
            _userRepository = userRepository;
        }

        public async Task CheckMaxUserCountAsync(int tenantId)
        {
            var maxUserCount = (await _featureChecker.GetValueAsync(tenantId, AppFeatures.MaxUserCount)).To<int>();
            if (maxUserCount <= 0)
            {
                return;
            }

            var currentUserCount = await _userRepository.CountAsync();
            if (currentUserCount >= maxUserCount)
            {
                throw new UserFriendlyException(L("MaximumUserCount_Error_Message"), L("MaximumUserCount_Error_Detail", maxUserCount));
            }
        }
    }
}