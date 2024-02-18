using System;
using System.ComponentModel.DataAnnotations;

namespace MyTrainingV1231AngularDemo.Configuration.Host.Dto
{
    public class UserPasswordSettingsEditDto
    {
        public bool EnableCheckingLastXPasswordWhenPasswordChange { get; set; }

        public int CheckingLastXPasswordCount { get; set; }
        
        public bool EnablePasswordExpiration { get; set; }

        public int PasswordExpirationDayCount { get; set; }
        
        [Range(1, Int32.MaxValue)]
        public int PasswordResetCodeExpirationHours { get; set; }
    }
}