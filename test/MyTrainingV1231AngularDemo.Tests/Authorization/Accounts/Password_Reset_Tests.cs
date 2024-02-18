using System;
using System.Threading.Tasks;
using Abp.Localization;
using Abp.Timing;
using Abp.UI;
using Castle.MicroKernel.Registration;
using Microsoft.AspNetCore.Identity;
using MyTrainingV1231AngularDemo.Authorization.Accounts;
using MyTrainingV1231AngularDemo.Authorization.Accounts.Dto;
using MyTrainingV1231AngularDemo.Authorization.Users;
using NSubstitute;
using Shouldly;
using Xunit;


namespace MyTrainingV1231AngularDemo.Tests.Authorization.Accounts
{
    // ReSharper disable once InconsistentNaming
    public class Password_Reset_Tests : AppTestBase
    {
        [Fact]
        public async Task Should_Reset_Password()
        {
            //Arrange

            var user = await GetCurrentUserAsync();

            string passResetCode = null;

            var fakeUserEmailer = Substitute.For<IUserEmailer>();
            var localUser = user;
            fakeUserEmailer.SendPasswordResetLinkAsync(Arg.Any<User>(), Arg.Any<string>()).Returns(callInfo =>
            {
                var calledUser = callInfo.Arg<User>();
                calledUser.EmailAddress.ShouldBe(localUser.EmailAddress);
                passResetCode =
                    calledUser.PasswordResetCode; //Getting the password reset code sent to the email address
                return Task.CompletedTask;
            });

            LocalIocManager.IocContainer.Register(Component.For<IUserEmailer>().Instance(fakeUserEmailer).IsDefault());

            var accountAppService = Resolve<IAccountAppService>();

            //Act

            await accountAppService.SendPasswordResetCode(
                new SendPasswordResetCodeInput
                {
                    EmailAddress = user.EmailAddress
                }
            );

            await accountAppService.ResetPassword(
                new ResetPasswordInput
                {
                    Password = "New@Passw0rd",
                    ResetCode = passResetCode,
                    UserId = user.Id,
                    ExpireDate = Clock.Now.AddDays(1)
                }
            );

            //Assert

            user = await GetCurrentUserAsync();
            LocalIocManager
                .Resolve<IPasswordHasher<User>>()
                .VerifyHashedPassword(user, user.Password, "New@Passw0rd")
                .ShouldBe(PasswordVerificationResult.Success);
        }

        [Fact]
        public async Task Should_Not_Reset_Password_When_ResetCode_Is_Expired()
        {
            var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            {
                var user = await GetCurrentUserAsync();
                var expireDate = Clock.Now.AddDays(-1);
                string resetCode = null;

                var fakeUserMailer = Substitute.For<IUserEmailer>();
                fakeUserMailer.SendPasswordResetLinkAsync(Arg.Any<User>(), Arg.Any<string>()).Returns(callInfo =>
                {
                    var calledUser = callInfo.Arg<User>();
                    calledUser.EmailAddress.ShouldBe(user.EmailAddress);
                    resetCode = calledUser.PasswordResetCode; //Getting the confirmation code sent to the email address
                    return Task.CompletedTask;
                });

                LocalIocManager.IocContainer.Register(
                    Component.For<IUserEmailer>().Instance(fakeUserMailer).IsDefault());

                var accountAppService = Resolve<IAccountAppService>();

                await accountAppService.SendPasswordResetCode(new SendPasswordResetCodeInput
                {
                    EmailAddress = user.EmailAddress
                });

                await accountAppService.ResetPassword(new ResetPasswordInput
                {
                    UserId = user.Id,
                    ResetCode = resetCode,
                    Password = "123qwe",
                    ExpireDate = expireDate
                });

                user.PasswordResetCode.ShouldBe(null);
            });

            var localizationManager = Resolve<ILocalizationManager>();

            exception.Message.ShouldContain(localizationManager.GetString(MyTrainingV1231AngularDemoConsts.LocalizationSourceName,
                "PasswordResetLinkExpired"));
        }
    }
}