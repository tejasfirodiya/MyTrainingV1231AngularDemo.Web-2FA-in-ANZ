using System.Threading.Tasks;
using Abp.Application.Services;
using MyTrainingV1231AngularDemo.MultiTenancy.Payments.PayPal.Dto;

namespace MyTrainingV1231AngularDemo.MultiTenancy.Payments.PayPal
{
    public interface IPayPalPaymentAppService : IApplicationService
    {
        Task ConfirmPayment(long paymentId, string paypalOrderId);

        PayPalConfigurationDto GetConfiguration();
    }
}
