using MyTrainingV1231AngularDemo.Editions.Dto;

namespace MyTrainingV1231AngularDemo.MultiTenancy.Payments.Dto
{
    public class PaymentInfoDto
    {
        public EditionSelectDto Edition { get; set; }

        public decimal AdditionalPrice { get; set; }

        public bool IsLessThanMinimumUpgradePaymentAmount()
        {
            return AdditionalPrice < MyTrainingV1231AngularDemoConsts.MinimumUpgradePaymentAmount;
        }
    }
}
