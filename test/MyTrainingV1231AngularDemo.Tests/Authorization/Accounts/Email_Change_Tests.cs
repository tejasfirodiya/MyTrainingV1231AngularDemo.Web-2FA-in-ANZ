using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Abp;
using Abp.Runtime.Security;
using Castle.MicroKernel.Registration;
using MyTrainingV1231AngularDemo.Authorization.Accounts;
using MyTrainingV1231AngularDemo.Authorization.Accounts.Dto;
using MyTrainingV1231AngularDemo.Authorization.Users;
using MyTrainingV1231AngularDemo.Url;
using NSubstitute;
using Shouldly;
using Xunit;

namespace MyTrainingV1231AngularDemo.Tests.Authorization.Accounts;

// ReSharper disable once InconsistentNaming
public class Email_Change_Tests : AppTestBase
{
    [Fact]
    public async Task Should_Change_Email()
    {
        //Arrange

        UsingDbContext(context =>
        {
            //Set IsEmailConfirmed to false to provide initial test case
            var currentUser = context.Users.Single(u => u.Id == AbpSession.UserId.Value);
            currentUser.IsEmailConfirmed = false;
        });

        var user = await GetCurrentUserAsync();
        user.IsEmailConfirmed.ShouldBeFalse();
        
        const string newEmailAddress = "example@gmail.com";
        
        var accountAppService = Resolve<IAccountAppService>();
        var userEmailer = Resolve<IUserEmailer>();
        var appUrlService = Resolve<IAppUrlService>();

        //Act

        await userEmailer.SendEmailChangeRequestLinkAsync(
            user,
            newEmailAddress,
            appUrlService.CreateEmailChangeRequestUrlFormat(AbpSession.TenantId)
        );

        await accountAppService.ChangeEmail(
            new ChangeEmailInput()
            {
                UserId = user.Id,
                EmailAddress = newEmailAddress,
                OldEmailAddress = user.EmailAddress
            }
        );

        //Assert

        user = await GetCurrentUserAsync();
        user.EmailAddress.ShouldBe(newEmailAddress);
    }
    
    [Fact]
    public async Task Should_Throw_Exception_Wrong_Old_Email()
    {
        //Arrange

        UsingDbContext(context =>
        {
            //Set IsEmailConfirmed to false to provide initial test case
            var currentUser = context.Users.Single(u => u.Id == AbpSession.UserId.Value);
            currentUser.IsEmailConfirmed = false;
        });

        var user = await GetCurrentUserAsync();
        user.IsEmailConfirmed.ShouldBeFalse();
        
        const string newEmailAddress = "example@gmail.com";
        
        var accountAppService = Resolve<IAccountAppService>();
        
        //Act & Assert

        await Should.ThrowAsync<AbpException>( async () =>
        {
            await accountAppService.ChangeEmail(
                new ChangeEmailInput()
                {
                    UserId = user.Id,
                    EmailAddress = newEmailAddress,
                    OldEmailAddress = "test@email.com"
                }
            );
        });
    }
}