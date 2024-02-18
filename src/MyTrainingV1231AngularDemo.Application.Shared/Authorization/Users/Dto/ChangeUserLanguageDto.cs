using System.ComponentModel.DataAnnotations;

namespace MyTrainingV1231AngularDemo.Authorization.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}
