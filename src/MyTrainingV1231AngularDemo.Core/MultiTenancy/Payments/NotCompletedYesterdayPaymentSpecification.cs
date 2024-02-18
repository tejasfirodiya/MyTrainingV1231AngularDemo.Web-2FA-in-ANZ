using System;
using System.Linq.Expressions;
using Abp.Specifications;
using Abp.Timing;

namespace MyTrainingV1231AngularDemo.MultiTenancy.Payments
{
    public class NotCompletedYesterdayPaymentSpecification: Specification<SubscriptionPayment>
    {
        public override Expression<Func<SubscriptionPayment, bool>> ToExpression()
        {
            var todaysDate = Clock.Now.Date;
            var yesterdaysDate = todaysDate.AddDays(-1).Date;

            return payment =>
                payment.Status == SubscriptionPaymentStatus.NotPaid &&
                payment.CreationTime >= yesterdaysDate &&
                payment.CreationTime < todaysDate;
        }
    }
}