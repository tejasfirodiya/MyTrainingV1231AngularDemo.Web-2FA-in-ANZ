using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MyTrainingV1231AngularDemo.Authentication.TwoFactor.Google;
using MyTrainingV1231AngularDemo.Authorization.Users;
using MyTrainingV1231AngularDemo.Authorization.Users.Profile;
using MyTrainingV1231AngularDemo.Authorization.Users.Profile.Dto;
using MyTrainingV1231AngularDemo.Test.Base;
using Shouldly;
using Xunit;

// TODO: Add test for two factor authentication.

namespace MyTrainingV1231AngularDemo.Tests.Authorization.Users
{
    // ReSharper disable once InconsistentNaming
    public class ProfileAppService_Tests : AppTestBase
    {
        private readonly IProfileAppService _profileAppService;
        private readonly GoogleTwoFactorAuthenticateService _googleTwoFactorAuthenticateService;

        public ProfileAppService_Tests()
        {
            _profileAppService = Resolve<IProfileAppService>();
            _googleTwoFactorAuthenticateService = Resolve<GoogleTwoFactorAuthenticateService>();
        }

        [Fact]
        public async Task UpdateGoogleAuthenticatorKey_Test()
        {
            var currentUser = await GetCurrentUserAsync();

            //Assert
            currentUser.GoogleAuthenticatorKey.ShouldBeNull();

            //Act
            var result = await _profileAppService.GenerateGoogleAuthenticatorKey();
            await _profileAppService.UpdateGoogleAuthenticatorKey(new UpdateGoogleAuthenticatorKeyInput
            {
                GoogleAuthenticatorKey = result.GoogleAuthenticatorKey,
                AuthenticatorCode = _googleTwoFactorAuthenticateService.GetCurrentPins(result.GoogleAuthenticatorKey, TimeSpan.FromMinutes(5)).First()
            });

            currentUser = await GetCurrentUserAsync();

            //Assert
            currentUser.GoogleAuthenticatorKey.ShouldNotBeNull();
        }

        [Fact]
        public async Task GetUserProfileForEdit_Test()
        {
            //Act
            var output = await _profileAppService.GetCurrentUserProfileForEdit();

            //Assert
            var currentUser = await GetCurrentUserAsync();
            output.Name.ShouldBe(currentUser.Name);
            output.Surname.ShouldBe(currentUser.Surname);
            output.EmailAddress.ShouldBe(currentUser.EmailAddress);
        }

        [Fact]
        public async Task UpdateUserProfileForEdit_Test()
        {
            //Arrange
            var currentUser = await GetCurrentUserAsync();

            //Act
            await _profileAppService.UpdateCurrentUserProfile(
                new CurrentUserProfileEditDto
                {
                    EmailAddress = "test1@test.net",
                    Name = "modified name",
                    Surname = "modified surname",
                    UserName = currentUser.UserName
                });

            //Assert
            currentUser = await GetCurrentUserAsync();
            currentUser.Name.ShouldBe("modified name");
            currentUser.Surname.ShouldBe("modified surname");
        }

        [Fact]
        public async Task ChangePassword_Test()
        {
            //Act
            await _profileAppService.ChangePassword(
                new ChangePasswordInput
                {
                    CurrentPassword = "123qwe",
                    NewPassword = "2mF9d8Ac!5"
                });

            //Assert
            var currentUser = await GetCurrentUserAsync();

            LocalIocManager
                .Resolve<IPasswordHasher<User>>()
                .VerifyHashedPassword(currentUser, currentUser.Password, "2mF9d8Ac!5")
                .ShouldBe(PasswordVerificationResult.Success);
        } 
    }
}
