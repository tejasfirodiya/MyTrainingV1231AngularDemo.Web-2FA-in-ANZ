using System;
using Abp;
using Abp.Runtime.Validation;

namespace MyTrainingV1231AngularDemo.Configuration.Host.Dto
{
    public class UserLockOutSettingsEditDto: ICustomValidate
    {
        public bool IsEnabled { get; set; }

        public int? MaxFailedAccessAttemptsBeforeLockout { get; set; }

        public int? DefaultAccountLockoutSeconds { get; set; }
        
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (!IsEnabled)
            {
                return;
            }

            if (!MaxFailedAccessAttemptsBeforeLockout.HasValue)
            {
                throw new ArgumentNullException(nameof(MaxFailedAccessAttemptsBeforeLockout));
            }
            
            if (!DefaultAccountLockoutSeconds.HasValue)
            {
                throw new ArgumentNullException(nameof(DefaultAccountLockoutSeconds));
            }
        }
    }
}