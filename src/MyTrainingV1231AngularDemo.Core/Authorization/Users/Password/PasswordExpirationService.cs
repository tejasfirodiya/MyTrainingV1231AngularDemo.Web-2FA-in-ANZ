using System;
using System.Linq;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Timing;
using MyTrainingV1231AngularDemo.Configuration;
using MyTrainingV1231AngularDemo.MultiTenancy;

namespace MyTrainingV1231AngularDemo.Authorization.Users.Password
{
    public class PasswordExpirationService : MyTrainingV1231AngularDemoDomainServiceBase, IPasswordExpirationService
    {
        private readonly IRepository<RecentPassword, Guid> _recentPasswordRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Tenant> _tenantRepository;

        public PasswordExpirationService(
            IRepository<RecentPassword, Guid> recentPasswordRepository,
            IUserRepository userRepository,
            IRepository<Tenant> tenantRepository)
        {
            _recentPasswordRepository = recentPasswordRepository;
            _userRepository = userRepository;
            _tenantRepository = tenantRepository;
        }

        public void ForcePasswordExpiredUsersToChangeTheirPassword()
        {
            var isEnabled = SettingManager.GetSettingValueForApplication<bool>(
                AppSettings.UserManagement.Password.EnablePasswordExpiration
            );

            if (!isEnabled)
            {
                return;
            }

            // check host users 
            ForcePasswordExpiredUsersToChangeTheirPasswordInternal(null);

            // check tenants
            var tenantIds = _tenantRepository.GetAll().Select(tenant => tenant.Id).ToList();
            foreach (var tenantId in tenantIds)
            {
                ForcePasswordExpiredUsersToChangeTheirPasswordInternal(tenantId);
            }
        }

        private void ForcePasswordExpiredUsersToChangeTheirPasswordInternal(int? tenantId)
        {
            using (CurrentUnitOfWork.SetTenantId(tenantId))
            {
                var passwordExpirationDayCount = SettingManager.GetSettingValueForApplication<int>(
                    AppSettings.UserManagement.Password.PasswordExpirationDayCount
                );

                var passwordExpireDate = Clock.Now.AddDays(-passwordExpirationDayCount).ToUniversalTime();

                // TODO: Query seems wrong !
                var passwordExpiredUsers = _userRepository.GetPasswordExpiredUserIds(passwordExpireDate);

                var separationCount = 1000;
                var separationLoopCount = passwordExpiredUsers.Count / separationCount + 1;

                for (int i = 0; i < separationLoopCount; i++)
                {
                    var userIdsToUpdate = passwordExpiredUsers.Skip(i * separationCount).Take(separationCount).ToList();
                    if (userIdsToUpdate.Count > 0)
                    {
                        _userRepository.UpdateUsersToChangePasswordOnNextLogin(userIdsToUpdate);
                    }
                }
            }
        }
    }
}
