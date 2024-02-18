using Abp.Events.Bus;

namespace MyTrainingV1231AngularDemo.MultiTenancy
{
    public class RecurringPaymentsEnabledEventData : EventData
    {
        public int TenantId { get; set; }
    }
}