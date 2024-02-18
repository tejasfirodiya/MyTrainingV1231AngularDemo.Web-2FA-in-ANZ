using System.Threading.Tasks;
using Abp.Application.Services;
using MyTrainingV1231AngularDemo.MultiTenancy.Payments.Dto;
using MyTrainingV1231AngularDemo.MultiTenancy.Payments.Stripe.Dto;

namespace MyTrainingV1231AngularDemo.MultiTenancy.Payments.Stripe
{
    public interface IStripePaymentAppService : IApplicationService
    {
        Task ConfirmPayment(StripeConfirmPaymentInput input);

        StripeConfigurationDto GetConfiguration();

        Task<SubscriptionPaymentDto> GetPaymentAsync(StripeGetPaymentInput input);

        Task<string> CreatePaymentSession(StripeCreatePaymentSessionInput input);
    }
}