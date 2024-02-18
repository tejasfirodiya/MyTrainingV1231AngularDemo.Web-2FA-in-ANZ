using System.Threading.Tasks;
using Abp.Webhooks;

namespace MyTrainingV1231AngularDemo.WebHooks
{
    public interface IWebhookEventAppService
    {
        Task<WebhookEvent> Get(string id);
    }
}
