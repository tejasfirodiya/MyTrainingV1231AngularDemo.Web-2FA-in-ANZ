using System.ComponentModel.DataAnnotations;

namespace MyTrainingV1231AngularDemo.Authorization.Accounts.Dto
{
    public class SendEmailActivationLinkInput
    {
        [Required]
        public string EmailAddress { get; set; }
    }
}