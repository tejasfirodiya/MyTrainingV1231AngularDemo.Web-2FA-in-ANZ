using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Timing;
using Microsoft.EntityFrameworkCore;
using MyTrainingV1231AngularDemo.Authorization.Users;
using MyTrainingV1231AngularDemo.Authorization.Users.Password;
using MyTrainingV1231AngularDemo.Configuration;
using Shouldly;
using Xunit;
using Z.EntityFramework.Plus;

namespace MyTrainingV1231AngularDemo.Tests.Authorization.Users
{
    public class PasswordExpiration_Tests : AppTestBase
    {
        private readonly IPasswordExpirationService _passwordExpirationDomainService;
        private readonly UserManager _userManager;
        private readonly SettingManager _settingManager;
        private readonly IRepository<RecentPassword, Guid> _recentPasswordRepository;
        private readonly IUserRepository _userRepository;

        public PasswordExpiration_Tests()
        {
            _passwordExpirationDomainService = Resolve<IPasswordExpirationService>();
            _userManager = Resolve<UserManager>();
            _settingManager = Resolve<SettingManager>();
            _recentPasswordRepository = Resolve<IRepository<RecentPassword, Guid>>();
            _userRepository = Resolve<IUserRepository>();
        }
        
        [Fact]
        public async Task Should_Create_Users_With_Given_Date()
        {
            var creationTime = DateTime.Now.AddYears(new Random().Next(-10, -1));

            var usersCreated = await CreateTestUserWithUpdatedCreationTime(creationTime, 10);
            foreach (var user in usersCreated)
            {
                user.CreationTime.ShouldBe(creationTime);
            }

            await UsingDbContextAsync(async context =>
            {
                var userIds = usersCreated.Select(u => u.Id).ToList();
                var users = await context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
                foreach (var user in users)
                {
                    user.CreationTime.ShouldBe(creationTime);
                }

                var userAccounts = await context.UserAccounts.Where(u => userIds.Contains(u.UserId)).ToListAsync();
                foreach (var userAccount in userAccounts)
                {
                    userAccount.CreationTime.ShouldBe(creationTime);
                }
            });
        }

        [Fact]
        public async Task Should_Force_Users_To_Change_Password_If_Setting_Is_Enabled()
        {
            await EnableResetPasswordFeature(2);

            var passwordExpiredUsers = await CreateTestUserWithUpdatedCreationTime(DateTime.Now.AddDays(-3), 10);
            var passwordNonExpiredUsers = await CreateTestUserWithUpdatedCreationTime(DateTime.Now.AddDays(-1), 10);

            //check if their ShouldChangePasswordOnNextLogin is false
            await UsingDbContextAsync(async context =>
            {
                var userIds = passwordExpiredUsers.Select(u => u.Id).ToList();
                var nonExpiredUserIds = passwordNonExpiredUsers.Select(u => u.Id).ToList();
                userIds.AddRange(nonExpiredUserIds);

                var test = await context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
                foreach (var user in test)
                {
                    user.ShouldChangePasswordOnNextLogin.ShouldBeFalse();
                }
            });

            //expire users password
            WithUnitOfWork(() =>
            {
                _passwordExpirationDomainService.ForcePasswordExpiredUsersToChangeTheirPassword();
            });

            //check ShouldChangePasswordOnNextLogin of users
            await UsingDbContextAsync(async context =>
            {
                var expiredUserIds = passwordExpiredUsers.Select(u => u.Id).ToList();
                var expiredUsers = await context.Users.Where(u => expiredUserIds.Contains(u.Id)).ToListAsync();
                foreach (var user in expiredUsers)
                {
                    user.ShouldChangePasswordOnNextLogin.ShouldBeTrue();
                }

                var nonExpiredUserIds = passwordNonExpiredUsers.Select(u => u.Id).ToList();
                var nonExpiredUsers = await context.Users.Where(u => nonExpiredUserIds.Contains(u.Id)).ToListAsync();
                foreach (var user in nonExpiredUsers)
                {
                    user.ShouldChangePasswordOnNextLogin.ShouldBeFalse();
                }
            });
        }

        [Fact]
        public async Task Should_Not_Force_Users_To_Change_Password_If_Setting_Is_Not_Enabled()
        {
            await _settingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.Password.EnablePasswordExpiration, "false"
            );

            var passwordExpiredUsers = await CreateTestUserWithUpdatedCreationTime(DateTime.Now.AddDays(-3), 10);
            var passwordNonExpiredUsers = await CreateTestUserWithUpdatedCreationTime(DateTime.Now.AddDays(-1), 10);

            //check if their ShouldChangePasswordOnNextLogin is false
            await UsingDbContextAsync(async context =>
            {
                var userIds = passwordExpiredUsers.Select(u => u.Id).ToList();
                var nonExpiredUserIds = passwordNonExpiredUsers.Select(u => u.Id).ToList();
                userIds.AddRange(nonExpiredUserIds);

                var test = await context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
                foreach (var user in test)
                {
                    user.ShouldChangePasswordOnNextLogin.ShouldBeFalse();
                }
            });

            //expire users password
            WithUnitOfWork(() =>
            {
                _passwordExpirationDomainService.ForcePasswordExpiredUsersToChangeTheirPassword();
            });

            //check ShouldChangePasswordOnNextLogin of users
            await UsingDbContextAsync(async context =>
            {
                var expiredUserIds = passwordExpiredUsers.Select(u => u.Id).ToList();
                var expiredUsers = await context.Users.Where(u => expiredUserIds.Contains(u.Id)).ToListAsync();
                foreach (var user in expiredUsers)
                {
                    user.ShouldChangePasswordOnNextLogin
                        .ShouldBeFalse(); //since setting is not enabled, it should not be changed
                }

                var nonExpiredUserIds = passwordNonExpiredUsers.Select(u => u.Id).ToList();
                var nonExpiredUsers = await context.Users.Where(u => nonExpiredUserIds.Contains(u.Id)).ToListAsync();
                foreach (var user in nonExpiredUsers)
                {
                    user.ShouldChangePasswordOnNextLogin.ShouldBeFalse();
                }
            });
        }

        [Fact]
        public async Task Should_Create_RecentPassword_With_Given_Date()
        {
            var creationTime = DateTime.Now.AddYears(new Random().Next(-10, -1));

            var usersCreated = await CreateTestUserWithUpdatedCreationTime(creationTime, 1);
            var recentPassword =
                await CreateRecentPasswordForUserWithGivenCreationTime(usersCreated.First(), creationTime);

            await UsingDbContextAsync(async context =>
            {
                var insertedRecentPassword = await context.RecentPasswords.FindAsync(recentPassword.Id);
                insertedRecentPassword.CreationTime.ShouldBe(creationTime);
            });
        }

        [Fact]
        public async Task Should_Not_Force_Users_To_Change_Password_If_Recent_Password_Exist_And_Not_Expired()
        {
            await EnableResetPasswordFeature(2);

            var passwordExpiredUsers = await CreateTestUserWithUpdatedCreationTime(DateTime.Now.AddDays(-3), 10);

            //check if their ShouldChangePasswordOnNextLogin is false
            await UsingDbContextAsync(async context =>
            {
                var userIds = passwordExpiredUsers.Select(u => u.Id).ToList();
                var users = await context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
                foreach (var user in users)
                {
                    user.ShouldChangePasswordOnNextLogin.ShouldBeFalse();
                }
            });

            //add recent password to a user
            var userWithRecentPassword = passwordExpiredUsers.First();
            await CreateRecentPasswordForUserWithGivenCreationTime(userWithRecentPassword, DateTime.Now);

            //expire users passwords
            WithUnitOfWork(() =>
            {
                _passwordExpirationDomainService.ForcePasswordExpiredUsersToChangeTheirPassword();
            });

            //check ShouldChangePasswordOnNextLogin of users, userWithRecentPassword's value must be false, others must be true
            await UsingDbContextAsync(async context =>
            {
                passwordExpiredUsers.Remove(userWithRecentPassword);
                var expiredUserIds = passwordExpiredUsers.Select(u => u.Id).ToList();
                var expiredUsers = await context.Users.Where(u => expiredUserIds.Contains(u.Id)).ToListAsync();
                foreach (var user in expiredUsers)
                {
                    user.ShouldChangePasswordOnNextLogin.ShouldBeTrue(); //others will have to change their password
                }

                var nonExpiredUser = await context.Users.FindAsync(userWithRecentPassword.Id);
                nonExpiredUser.ShouldChangePasswordOnNextLogin
                    .ShouldBeFalse(); //user with not expired recentpassword will not have to change password
            });
        }

        [Fact]
        public async Task Should_Force_Users_To_Change_Password_If_Recent_Password_Exist_But_Expired()
        {
            await EnableResetPasswordFeature(2);

            var passwordExpiredUsers = await CreateTestUserWithUpdatedCreationTime(DateTime.Now.AddDays(-3), 10);

            //check if their ShouldChangePasswordOnNextLogin is false
            await UsingDbContextAsync(async context =>
            {
                var userIds = passwordExpiredUsers.Select(u => u.Id).ToList();
                var users = await context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
                foreach (var user in users)
                {
                    user.ShouldChangePasswordOnNextLogin.ShouldBeFalse();
                }
            });

            //add recent password to a user
            var userWithRecentPassword = passwordExpiredUsers.First();
            await CreateRecentPasswordForUserWithGivenCreationTime(userWithRecentPassword, DateTime.Now.AddDays(-3));

            //expire users passwords
            WithUnitOfWork(() =>
            {
                _passwordExpirationDomainService.ForcePasswordExpiredUsersToChangeTheirPassword();
            });

            //check ShouldChangePasswordOnNextLogin of users, userWithRecentPassword's value must be false, others must be true
            await UsingDbContextAsync(async context =>
            {
                passwordExpiredUsers.Remove(userWithRecentPassword);
                var expiredUserIds = passwordExpiredUsers.Select(u => u.Id).ToList();
                var expiredUsers = await context.Users.Where(u => expiredUserIds.Contains(u.Id)).ToListAsync();
                foreach (var user in expiredUsers)
                {
                    user.ShouldChangePasswordOnNextLogin.ShouldBeTrue(); //others will have to change their password
                }

                var nonExpiredUser = await context.Users.FindAsync(userWithRecentPassword.Id);
                nonExpiredUser.ShouldChangePasswordOnNextLogin
                    .ShouldBeTrue(); //user with expired recentpassword will have to change password also
            });
        }

        [Fact]
        public async Task
            Should_Not_Force_Users_To_Change_Password_If_Recent_Password_Exist_And_Not_Expired_With_Multiple_Recent_Password()
        {
            await EnableResetPasswordFeature(2);
            
            var passwordExpiredUsers = await CreateTestUserWithUpdatedCreationTime(DateTime.Now.AddDays(-3), 10);

            //check if their ShouldChangePasswordOnNextLogin is false
            await UsingDbContextAsync(async context =>
            {
                var userIds = passwordExpiredUsers.Select(u => u.Id).ToList();
                var users = await context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
                foreach (var user in users)
                {
                    user.ShouldChangePasswordOnNextLogin.ShouldBeFalse();
                }
            });

            //add recent password to a user
            var userWithRecentPassword = passwordExpiredUsers.First();
            await CreateRecentPasswordForUserWithGivenCreationTime(userWithRecentPassword,
                DateTime.Now); //not expired recent password
            await CreateRecentPasswordForUserWithGivenCreationTime(userWithRecentPassword,
                DateTime.Now.AddDays(-10)); //expired recent password

            //expire users passwords
            WithUnitOfWork(() =>
            {
                _passwordExpirationDomainService.ForcePasswordExpiredUsersToChangeTheirPassword();
            });

            //check ShouldChangePasswordOnNextLogin of users, userWithRecentPassword's value must be false, others must be true
            await UsingDbContextAsync(async context =>
            {
                passwordExpiredUsers.Remove(userWithRecentPassword);
                var expiredUserIds = passwordExpiredUsers.Select(u => u.Id).ToList();
                var expiredUsers = await context.Users.Where(u => expiredUserIds.Contains(u.Id)).ToListAsync();
                foreach (var user in expiredUsers)
                {
                    user.ShouldChangePasswordOnNextLogin.ShouldBeTrue(); //others will have to change their password
                }

                var nonExpiredUser = await context.Users.FindAsync(userWithRecentPassword.Id);
                nonExpiredUser.ShouldChangePasswordOnNextLogin
                    .ShouldBeFalse(); //user with not expired recentpassword will not have to change password
            });
        }

        [Fact]
        public async Task
            Should_Force_Users_To_Change_Password_If_Recent_Password_Exist_But_Expired_With_Multiple_Recent_Password()
        {
            await EnableResetPasswordFeature(2);

            var passwordExpiredUsers = await CreateTestUserWithUpdatedCreationTime(DateTime.Now.AddDays(-3), 10);

            //check if their ShouldChangePasswordOnNextLogin is false
            await UsingDbContextAsync(async context =>
            {
                var userIds = passwordExpiredUsers.Select(u => u.Id).ToList();
                var users = await context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
                foreach (var user in users)
                {
                    user.ShouldChangePasswordOnNextLogin.ShouldBeFalse();
                }
            });

            //add recent password to a user
            var userWithRecentPassword = passwordExpiredUsers.First();
            await CreateRecentPasswordForUserWithGivenCreationTime(userWithRecentPassword,
                DateTime.Now.AddDays(-3)); //expired recent password
            await CreateRecentPasswordForUserWithGivenCreationTime(userWithRecentPassword,
                DateTime.Now.AddDays(-10)); //expired recent password

            //expire users passwords
            WithUnitOfWork(() =>
            {
                _passwordExpirationDomainService.ForcePasswordExpiredUsersToChangeTheirPassword();
            });

            //check ShouldChangePasswordOnNextLogin of users, userWithRecentPassword's value must be false, others must be true
            await UsingDbContextAsync(async context =>
            {
                passwordExpiredUsers.Remove(userWithRecentPassword);
                var expiredUserIds = passwordExpiredUsers.Select(u => u.Id).ToList();
                var expiredUsers = await context.Users.Where(u => expiredUserIds.Contains(u.Id)).ToListAsync();
                foreach (var user in expiredUsers)
                {
                    user.ShouldChangePasswordOnNextLogin.ShouldBeTrue(); //others will have to change their password
                }

                var nonExpiredUser = await context.Users.FindAsync(userWithRecentPassword.Id);
                nonExpiredUser.ShouldChangePasswordOnNextLogin
                    .ShouldBeTrue(); //user with expired recentpassword will have to change password also
            });
        }

        [Fact]
        public async Task Should_Get_Delete_User_Created_Before_Password_Reset_Days()
        {
            await EnableResetPasswordFeature(2);
            
            // User without recent password
            var user = CreateUserEntity("john.doe", "John", "Doe", "john@doe.com");
            user.CreationTime = Clock.Now.AddDays(-5);
            await _userManager.CreateAsync(user);

            await _userManager.DeleteAsync(user);
            
            var usersToResetPassword = _userRepository.GetPasswordExpiredUserIds(DateTime.Now.AddDays(-2));
            usersToResetPassword.Count.ShouldBe(0);
        }
        
        [Fact]
        public async Task Should_Get_User_Created_Before_Password_Reset_Days()
        {
            await EnableResetPasswordFeature(2);
            
            // User without recent password
            var user = CreateUserEntity("john.doe", "John", "Doe", "john@doe.com");
            user.CreationTime = Clock.Now.AddDays(-5);
            await _userManager.CreateAsync(user);

            var usersToResetPassword = _userRepository.GetPasswordExpiredUserIds(DateTime.Now.AddDays(-2));
            usersToResetPassword.Count.ShouldBe(1);
            usersToResetPassword.First().ShouldBe(user.Id);
        }
        
        [Fact]
        public async Task Should_Get_User_Created_Before_Password_Reset_Days_And_Resetted_Password_Before_Password_Reset_Days()
        {
            await EnableResetPasswordFeature(2);
            
            // User without recent password
            var user = CreateUserEntity("john.doe", "John", "Doe", "john@doe.com");
            user.CreationTime = Clock.Now.AddDays(-5);
            await _userManager.CreateAsync(user);

            await _recentPasswordRepository.InsertAsync(new RecentPassword()
            {
                Password = "123qwe",
                TenantId = user.TenantId,
                UserId = user.Id,
                CreationTime = Clock.Now.AddDays(-4)
            });

            var usersToResetPassword = _userRepository.GetPasswordExpiredUserIds(DateTime.Now.AddDays(-2));
            usersToResetPassword.Count.ShouldBe(1);
            usersToResetPassword.First().ShouldBe(user.Id);
        }
        
        [Fact]
        public async Task Should_Not_Get_User_Created_Before_Password_Reset_Days_And_Resetted_Password_After_Password_Reset_Days()
        {
            await EnableResetPasswordFeature(2);
            
            // User without recent password
            var user = CreateUserEntity("john.doe", "John", "Doe", "john@doe.com");
            user.CreationTime = Clock.Now.AddDays(-5);
            await _userManager.CreateAsync(user);

            await _recentPasswordRepository.InsertAsync(new RecentPassword()
            {
                Password = "123qwe",
                TenantId = user.TenantId,
                UserId = user.Id,
                CreationTime = Clock.Now
            });

            var usersToResetPassword = _userRepository.GetPasswordExpiredUserIds(DateTime.Now.AddDays(-2));
            usersToResetPassword.Count.ShouldBe(0);
        }

        // TEST Users without RecentPassword record
        // GetPasswordExpiredUserIds

        // TEST
        // UpdateUsersToChangePasswordOnNextLogin
        
        private async Task EnableResetPasswordFeature(int passwordExpirationDayCount)
        {
            await _settingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.Password.EnablePasswordExpiration, "true"
            );

            await _settingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.Password.PasswordExpirationDayCount, passwordExpirationDayCount.ToString()
            );
        }
        
        User CreateUserEntity(string userName, string name, string surname, string emailAddress)
        {
            var user = new User
            {
                EmailAddress = emailAddress,
                IsEmailConfirmed = true,
                Name = name,
                Surname = surname,
                UserName = userName,
                Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==", //123qwe
                TenantId = AbpSession.TenantId,
                ShouldChangePasswordOnNextLogin = false,
                Permissions = new List<UserPermissionSetting>
                {
                    new UserPermissionSetting
                        { Name = "test.permission1", IsGranted = true, TenantId = AbpSession.TenantId },
                    new UserPermissionSetting
                        { Name = "test.permission2", IsGranted = true, TenantId = AbpSession.TenantId },
                    new UserPermissionSetting
                        { Name = "test.permission3", IsGranted = false, TenantId = AbpSession.TenantId },
                    new UserPermissionSetting
                        { Name = "test.permission4", IsGranted = false, TenantId = AbpSession.TenantId }
                }
            };

            user.SetNormalizedNames();

            return user;
        }

        private async Task<List<User>> CreateTestUserWithUpdatedCreationTime(DateTime creationTime, int loop)
        {
            var rnd = new Random();
            var users = new List<User>();

            await WithUnitOfWorkAsync(async () =>
            {
                var rndText = rnd.Next(Int32.MaxValue);
                for (int i = 0; i < loop; i++)
                {
                    var randText = "Test_" + rndText + "_" + i;
                    var user = CreateUserEntity(randText, randText + "name", randText + "surname", randText + "@h.c");
                    await _userManager.CreateAsync(user);
                    user.CreationTime = creationTime;
                    users.Add(user);
                }
            });

            await UsingDbContextAsync(async (context) =>
            {
                var userIds = users.Select(u => u.Id).ToList();
                await context.UserAccounts.Where(account => userIds.Contains(account.UserId))
                    .UpdateAsync(u =>
                        new UserAccount()
                        {
                            CreationTime = creationTime
                        }
                    );
            });

            return users;
        }

        private async Task<RecentPassword> CreateRecentPasswordForUserWithGivenCreationTime(User user,
            DateTime creationTime)
        {
            var recentPassword = new RecentPassword()
            {
                TenantId = user.TenantId,
                UserId = user.Id,
                Password = user.Password
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _recentPasswordRepository.InsertAndGetIdAsync(recentPassword);
                recentPassword.CreationTime = creationTime;
            });

            return recentPassword;
        }
    }
}
