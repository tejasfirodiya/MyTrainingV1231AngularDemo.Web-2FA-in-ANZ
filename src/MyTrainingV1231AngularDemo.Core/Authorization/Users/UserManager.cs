using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Organizations;
using Abp.Runtime.Caching;
using Abp.Threading;
using Abp.UI;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyTrainingV1231AngularDemo.Authorization.Roles;
using MyTrainingV1231AngularDemo.Configuration;
using MyTrainingV1231AngularDemo.Security;

namespace MyTrainingV1231AngularDemo.Authorization.Users
{
    /// <summary>
    /// User manager.
    /// Used to implement domain logic for users.
    /// Extends <see cref="AbpUserManager{TRole,TUser}"/>.
    /// </summary>
    public class UserManager : AbpUserManager<Role, User>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ILocalizationManager _localizationManager;
        private readonly ISettingManager _settingManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRepository<RecentPassword, Guid> _recentPasswords;

        public UserManager(
            UserStore userStore,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager> logger,
            RoleManager roleManager,
            IPermissionManager permissionManager,
            IUnitOfWorkManager unitOfWorkManager,
            ICacheManager cacheManager,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IOrganizationUnitSettings organizationUnitSettings,
            ISettingManager settingManager,
            ILocalizationManager localizationManager,
            IRepository<RecentPassword, Guid> recentPasswords,
            IRepository<UserLogin, long> userLoginRepository)
            : base(
                roleManager,
                userStore,
                optionsAccessor,
                passwordHasher,
                userValidators,
                passwordValidators,
                keyNormalizer,
                errors,
                services,
                logger,
                permissionManager,
                unitOfWorkManager,
                cacheManager,
                organizationUnitRepository,
                userOrganizationUnitRepository,
                organizationUnitSettings,
                settingManager,
                userLoginRepository)
        {
            _passwordHasher = passwordHasher;
            _unitOfWorkManager = unitOfWorkManager;
            _settingManager = settingManager;
            _localizationManager = localizationManager;
            _recentPasswords = recentPasswords;
        }

        public virtual async Task<User> GetUserOrNullAsync(UserIdentifier userIdentifier)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(userIdentifier.TenantId))
                {
                    return await FindByIdAsync(userIdentifier.UserId.ToString());
                }
            });
        }

        public User GetUserOrNull(UserIdentifier userIdentifier)
        {
            return AsyncHelper.RunSync(() => GetUserOrNullAsync(userIdentifier));
        }

        public async Task<User> GetUserAsync(UserIdentifier userIdentifier)
        {
            var user = await GetUserOrNullAsync(userIdentifier);
            if (user == null)
            {
                throw new Exception("There is no user: " + userIdentifier);
            }

            return user;
        }

        public User GetUser(UserIdentifier userIdentifier)
        {
            return AsyncHelper.RunSync(() => GetUserAsync(userIdentifier));
        }

        public override Task<IdentityResult> SetRolesAsync(User user, string[] roleNames)
        {
            if (user.UserName != AbpUserBase.AdminUserName)
            {
                return base.SetRolesAsync(user, roleNames);
            }

            // Always keep admin role for admin user
            var roles = roleNames.ToList();
            roles.Add(StaticRoleNames.Host.Admin);
            roleNames = roles.ToArray();

            return base.SetRolesAsync(user, roleNames);
        }

        public override async Task SetGrantedPermissionsAsync(User user, IEnumerable<Permission> permissions)
        {
            CheckPermissionsToUpdate(user, permissions);

            await base.SetGrantedPermissionsAsync(user, permissions);
        }

        public async Task<string> CreateRandomPassword()
        {
            var passwordComplexitySetting = new PasswordComplexitySetting
            {
                RequireDigit = await _settingManager.GetSettingValueAsync<bool>(
                    AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit
                ),
                RequireLowercase = await _settingManager.GetSettingValueAsync<bool>(
                    AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase
                ),
                RequireNonAlphanumeric = await _settingManager.GetSettingValueAsync<bool>(
                    AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric
                ),
                RequireUppercase = await _settingManager.GetSettingValueAsync<bool>(
                    AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase
                ),
                RequiredLength = await _settingManager.GetSettingValueAsync<int>(
                    AbpZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength
                )
            };

            var upperCaseLetters = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
            var lowerCaseLetters = "abcdefghijkmnopqrstuvwxyz";
            var digits = "0123456789";
            var nonAlphanumerics = "!@$?_-";

            string[] randomChars =
            {
                upperCaseLetters,
                lowerCaseLetters,
                digits,
                nonAlphanumerics
            };

            var rand = new Random(Environment.TickCount);
            var chars = new List<char>();

            if (passwordComplexitySetting.RequireUppercase)
            {
                chars.Insert(rand.Next(0, chars.Count),
                    upperCaseLetters[rand.Next(0, upperCaseLetters.Length)]
                );
            }

            if (passwordComplexitySetting.RequireLowercase)
            {
                chars.Insert(rand.Next(0, chars.Count),
                    lowerCaseLetters[rand.Next(0, lowerCaseLetters.Length)]
                );
            }

            if (passwordComplexitySetting.RequireDigit)
            {
                chars.Insert(rand.Next(0, chars.Count),
                    digits[rand.Next(0, digits.Length)]
                );
            }

            if (passwordComplexitySetting.RequireNonAlphanumeric)
            {
                chars.Insert(rand.Next(0, chars.Count),
                    nonAlphanumerics[rand.Next(0, nonAlphanumerics.Length)]
                );
            }

            for (var i = chars.Count; i < passwordComplexitySetting.RequiredLength; i++)
            {
                var rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]
                );
            }

            return new string(chars.ToArray());
        }

        private void CheckPermissionsToUpdate(User user, IEnumerable<Permission> permissions)
        {
            if (user.Name == AbpUserBase.AdminUserName &&
                (!permissions.Any(p => p.Name == AppPermissions.Pages_Administration_Roles_Edit) ||
                 !permissions.Any(p => p.Name == AppPermissions.Pages_Administration_Users_ChangePermissions)))
            {
                throw new UserFriendlyException(L("YouCannotRemoveUserRolePermissionsFromAdminUser"));
            }
        }

        private new string L(string name)
        {
            return _localizationManager.GetString(MyTrainingV1231AngularDemoConsts.LocalizationSourceName, name);
        }

        protected string L(string name, params object[] args) => string.Format(L(name), args);

        public override async Task<IdentityResult> ChangePasswordAsync(User user, string newPassword)
        {
            await CheckRecentPasswordsIfNeeded(user, newPassword);

            var result = await base.ChangePasswordAsync(user, newPassword);

            await StoreRecentPasswordIfNeeded(user, result);

            return result;
        }

        public override async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword,
            string newPassword)
        {
            await CheckRecentPasswordsIfNeeded(user, currentPassword, newPassword);

            var result = await base.ChangePasswordAsync(user, currentPassword, newPassword);

            await StoreRecentPasswordIfNeeded(user, result);

            return result;
        }

        private Task CheckRecentPasswordsIfNeeded(User user, string newPassword)
        {
            return CheckRecentPasswordsIfNeededInternal(user, user.Password, newPassword);
        }

        private Task CheckRecentPasswordsIfNeeded(User user, string currentPassword, string newPassword)
        {
            var currentPasswordHash = _passwordHasher.HashPassword(user, currentPassword);

            return CheckRecentPasswordsIfNeededInternal(user, currentPasswordHash, newPassword);
        }

        private async Task CheckRecentPasswordsIfNeededInternal(
            User user,
            string currentPasswordHash,
            string newPassword)
        {
            var isCheckingLastXPasswordEnabled = await _settingManager.GetSettingValueAsync<bool>(
                AppSettings.UserManagement.Password.EnableCheckingLastXPasswordWhenPasswordChange
            );

            if (!isCheckingLastXPasswordEnabled)
            {
                return;
            }

            var newPasswordAndCurrentPasswordVerificationResult = _passwordHasher.VerifyHashedPassword(
                user,
                currentPasswordHash,
                newPassword
            );

            var checkingLastXPasswordCount = await _settingManager.GetSettingValueAsync<int>(
                AppSettings.UserManagement.Password
                    .CheckingLastXPasswordCount
            );

            if (newPasswordAndCurrentPasswordVerificationResult != PasswordVerificationResult.Failed)
            {
                throw new UserFriendlyException(
                    L("NewPasswordMustBeDifferentThenLastXPassword", checkingLastXPasswordCount)
                );
            }

            var recentPasswords = await _recentPasswords.GetAll()
                .Where(rp => rp.UserId == user.Id)
                .OrderByDescending(rp => rp.CreationTime)
                .Take(checkingLastXPasswordCount)
                .ToListAsync();

            foreach (var recentPassword in recentPasswords)
            {
                var recentPasswordVerificationResult = _passwordHasher.VerifyHashedPassword(
                    user,
                    recentPassword.Password,
                    newPassword
                );

                if (recentPasswordVerificationResult != PasswordVerificationResult.Failed)
                {
                    throw new UserFriendlyException(
                        L("NewPasswordMustBeDifferentThenLastXPassword", checkingLastXPasswordCount)
                    );
                }
            }
        }

        private async Task StoreRecentPasswordIfNeeded(User user, IdentityResult result)
        {
            if (!result.Succeeded)
            {
                return;
            }

            var isCheckingLastXPasswordEnabled = await _settingManager.GetSettingValueAsync<bool>(
                AppSettings.UserManagement.Password.EnableCheckingLastXPasswordWhenPasswordChange
            );

            if (!isCheckingLastXPasswordEnabled)
            {
                return;
            }

            var recentPassword = new RecentPassword
            {
                Password = user.Password,
                UserId = user.Id,
                TenantId = user.TenantId
            };

            await _recentPasswords.InsertAsync(recentPassword);
        }
    }
}
