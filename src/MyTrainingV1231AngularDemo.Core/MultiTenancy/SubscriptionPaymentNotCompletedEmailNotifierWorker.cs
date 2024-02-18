using System;
using System.Linq;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using MyTrainingV1231AngularDemo.Authorization.Users;
using MyTrainingV1231AngularDemo.MultiTenancy.Payments;

namespace MyTrainingV1231AngularDemo.MultiTenancy
{
    public class SubscriptionPaymentNotCompletedEmailNotifierWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int CheckPeriodAsMilliseconds = 1 * 60 * 60 * 1000 * 24; //1 day

        private readonly UserEmailer _userEmailer;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ISubscriptionPaymentRepository _subscriptionPaymentRepository;
        private readonly IPaymentUrlGenerator _paymentUrlGenerator;

        public SubscriptionPaymentNotCompletedEmailNotifierWorker(
            AbpTimer timer,
            UserEmailer userEmailer,
            IUnitOfWorkManager unitOfWorkManager,
            ISubscriptionPaymentRepository subscriptionPaymentRepository,
            IPaymentUrlGenerator paymentUrlGenerator) : base(timer)
        {
            _userEmailer = userEmailer;
            _unitOfWorkManager = unitOfWorkManager;
            _subscriptionPaymentRepository = subscriptionPaymentRepository;
            _paymentUrlGenerator = paymentUrlGenerator;

            Timer.Period = CheckPeriodAsMilliseconds;
            Timer.RunOnStart = true;

            LocalizationSourceName = MyTrainingV1231AngularDemoConsts.LocalizationSourceName;
        }

        protected override void DoWork()
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                var notCompletedPayments = _subscriptionPaymentRepository.GetAll()
                    .Where(new NotCompletedYesterdayPaymentSpecification().ToExpression())
                    .ToList();

                foreach (var notCompletedPayment in notCompletedPayments)
                {
                    try
                    {
                        var paymentUrl = _paymentUrlGenerator.CreatePaymentRequestUrl(notCompletedPayment);
                        _userEmailer.TryToSendPaymentNotCompletedEmail(notCompletedPayment.TenantId, paymentUrl);
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception.Message, exception);
                    }
                }
            });
        }
    }
}