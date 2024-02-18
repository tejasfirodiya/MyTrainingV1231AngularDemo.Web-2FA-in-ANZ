using Abp.Configuration;
using Abp.Net.Mail;
using Abp.Net.Mail.Smtp;
using Abp.Runtime.Security;

namespace MyTrainingV1231AngularDemo.Net.Emailing
{
    public class MyTrainingV1231AngularDemoSmtpEmailSenderConfiguration : SmtpEmailSenderConfiguration
    {
        public MyTrainingV1231AngularDemoSmtpEmailSenderConfiguration(ISettingManager settingManager) : base(settingManager)
        {

        }

        public override string Password => SimpleStringCipher.Instance.Decrypt(GetNotEmptySettingValue(EmailSettingNames.Smtp.Password));
    }
}