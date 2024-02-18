using System.Threading.Tasks;
using MyTrainingV1231AngularDemo.Authorization.Users;

namespace MyTrainingV1231AngularDemo.WebHooks
{
    public interface IAppWebhookPublisher
    {
        Task PublishTestWebhook();
    }
}
