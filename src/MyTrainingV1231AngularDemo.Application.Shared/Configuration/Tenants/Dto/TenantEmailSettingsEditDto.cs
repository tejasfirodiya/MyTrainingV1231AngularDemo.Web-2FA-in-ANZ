using Abp.Auditing;
using MyTrainingV1231AngularDemo.Configuration.Dto;

namespace MyTrainingV1231AngularDemo.Configuration.Tenants.Dto
{
    public class TenantEmailSettingsEditDto : EmailSettingsEditDto
    {
        public bool UseHostDefaultEmailSettings { get; set; }
    }
}