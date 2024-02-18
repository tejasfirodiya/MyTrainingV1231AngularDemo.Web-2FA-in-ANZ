using Abp.Dependency;
using Abp.UI;
using System.Text;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;
using SendGrid;
using MyTrainingV1231AngularDemo.Configuration;

namespace MyTrainingV1231AngularDemo.Emails
{
    public class UserEmailer : MyTrainingV1231AngularDemoAppServiceBase, IUserEmailer, ITransientDependency
    {
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IAppConfigurationWriter _appConfigurationWriter;

        //public UserEmailer(IConfigurationRoot appConfiguration)
        public UserEmailer(IAppConfigurationAccessor appConfigurationAccessor,
            IAppConfigurationWriter appConfigurationWriter)
        {
            //_appConfiguration = appConfiguration;
            _appConfiguration = appConfigurationAccessor.Configuration;
            _appConfigurationWriter = appConfigurationWriter;
        }

        public async Task ReplaceBodyAndSend(string emailAddress, string subject, StringBuilder emailTemplate)
        {
            try
            {
                var apiKey = _appConfiguration[$"Settings:{MyTrainingV1231AngularDemoConsts.ApiKeyName}"];

                var client = new SendGridClient(apiKey);

                var msg = new SendGridMessage()
                {
                    From = new EmailAddress("ashok.sinare@waiin.com"),
                    Subject = subject,
                    HtmlContent = emailTemplate.ToString(),
                };
              
                msg.AddTo(new EmailAddress(emailAddress));
                await client.SendEmailAsync(msg);
              
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("UserEmailer - ReplaceBodyAndSend" + ex.Message.ToString());
            }
        }

        public async Task SendEmail()
        {
            try
            {
               
                var emailTemplate = new StringBuilder();
                var mailMessages = new StringBuilder();
                emailTemplate.Append("Hello, <br/>");
                emailTemplate.Append("This is a test email. <br/>");
                emailTemplate.Append("Thanks, <br/>");
                emailTemplate.Append("Tejas Firodiya <br/>");
                await ReplaceBodyAndSend("tejas.firodiya@waiin.com", "Test email finbotx", emailTemplate);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("UserEmailer - SendEmail" + ex.Message.ToString());
            }

        }

    }
}
