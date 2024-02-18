using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.Timing;
using MyTrainingV1231AngularDemo.MultiTenancy.Payments;

namespace MyTrainingV1231AngularDemo.Sessions.Dto
{
    public class TenantLoginInfoDto : EntityDto
    {
        public string TenancyName { get; set; }

        public string Name { get; set; }

        public virtual Guid? DarkLogoId { get; set; }

        public virtual string DarkLogoFileType { get; set; }
        
        public virtual Guid? DarkLogoMinimalId { get; set; }

        public virtual string DarkLogoMinimalFileType { get; set; }
        
        public virtual Guid? LightLogoId { get; set; }

        public virtual string LightLogoFileType { get; set; }
        
        public virtual Guid? LightLogoMinimalId { get; set; }

        public virtual string LightLogoMinimalFileType { get; set; }

        public Guid? CustomCssId { get; set; }

        public DateTime? SubscriptionEndDateUtc { get; set; }

        public bool IsInTrialPeriod { get; set; }

        public SubscriptionPaymentType SubscriptionPaymentType { get; set; }

        public EditionInfoDto Edition { get; set; }
        
        public List<NameValueDto> FeatureValues { get; set; }

        public DateTime CreationTime { get; set; }

        public PaymentPeriodType PaymentPeriodType { get; set; }

        public string SubscriptionDateString { get; set; }

        public string CreationTimeString { get; set; }

        public TenantLoginInfoDto()
        {
            FeatureValues = new List<NameValueDto>();
        }
        
        public bool IsInTrial()
        {
            return IsInTrialPeriod;
        }

        public bool SubscriptionIsExpiringSoon(int subscriptionExpireNootifyDayCount)
        {
            if (SubscriptionEndDateUtc.HasValue)
            {
                return Clock.Now.ToUniversalTime().AddDays(subscriptionExpireNootifyDayCount) >= SubscriptionEndDateUtc.Value;
            }

            return false;
        }

        public int GetSubscriptionExpiringDayCount()
        {
            if (!SubscriptionEndDateUtc.HasValue)
            {
                return 0;
            }

            return Convert.ToInt32(SubscriptionEndDateUtc.Value.ToUniversalTime().Subtract(Clock.Now.ToUniversalTime()).TotalDays);
        }

        public bool HasRecurringSubscription()
        {
            return SubscriptionPaymentType != SubscriptionPaymentType.Manual;
        }
        
        public virtual bool HasLogo()
        {
            return (DarkLogoId != null && DarkLogoFileType != null) ||
                   (LightLogoId != null && LightLogoFileType != null);
        }
    }
}
