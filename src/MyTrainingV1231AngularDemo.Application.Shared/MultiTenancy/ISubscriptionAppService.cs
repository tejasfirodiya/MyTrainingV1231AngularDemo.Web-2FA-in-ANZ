using System.Threading.Tasks;
using Abp.Application.Services;

namespace MyTrainingV1231AngularDemo.MultiTenancy
{
    public interface ISubscriptionAppService : IApplicationService
    {
        Task DisableRecurringPayments();

        Task EnableRecurringPayments();
    }
}
