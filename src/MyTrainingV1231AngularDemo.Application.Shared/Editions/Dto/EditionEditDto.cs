using System.ComponentModel.DataAnnotations;

namespace MyTrainingV1231AngularDemo.Editions.Dto
{
    public class EditionEditDto
    {
        public int? Id { get; set; }

        [Required]
        public string DisplayName { get; set; }

        public int? ExpiringEditionId { get; set; }
    }
}