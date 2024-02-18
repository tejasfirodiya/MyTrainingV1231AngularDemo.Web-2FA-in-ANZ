using System.ComponentModel.DataAnnotations;

namespace MyTrainingV1231AngularDemo.Localization.Dto
{
    public class CreateOrUpdateLanguageInput
    {
        [Required]
        public ApplicationLanguageEditDto Language { get; set; }
    }
}