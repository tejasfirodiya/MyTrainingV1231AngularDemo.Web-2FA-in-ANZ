using Abp.Auditing;

namespace MyTrainingV1231AngularDemo.Configuration.Tenants.Dto
{
    public class LdapSettingsEditDto
    {
        public bool IsModuleEnabled { get; set; }

        public bool IsEnabled { get; set; }
        
        public string Domain { get; set; }
        
        public string UserName { get; set; }

        [DisableAuditing]
        public string Password { get; set; }

        public bool UseSsl { get; set; }

        public LdapSettingsEditDto()
        {
            UseSsl = false;
        }
    }
}