using Abp.Configuration;

namespace MyTrainingV1231AngularDemo.Timing.Dto
{
    public class GetTimezoneComboboxItemsInput
    {
        public SettingScopes DefaultTimezoneScope;

        public string SelectedTimezoneId { get; set; }
    }
}
