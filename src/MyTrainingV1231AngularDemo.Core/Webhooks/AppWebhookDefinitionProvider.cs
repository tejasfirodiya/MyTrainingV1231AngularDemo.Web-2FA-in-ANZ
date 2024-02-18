using Abp.Localization;
using Abp.Webhooks;

namespace MyTrainingV1231AngularDemo.WebHooks
{
    public class AppWebhookDefinitionProvider : WebhookDefinitionProvider
    {
        public override void SetWebhooks(IWebhookDefinitionContext context)
        {
            context.Manager.Add(new WebhookDefinition(
                name: AppWebHookNames.TestWebhook
            ));

            //Add your webhook definitions here 
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, MyTrainingV1231AngularDemoConsts.LocalizationSourceName);
        }
    }
}
