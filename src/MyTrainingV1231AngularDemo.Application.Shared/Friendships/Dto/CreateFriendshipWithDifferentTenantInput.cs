using System.ComponentModel.DataAnnotations;

namespace MyTrainingV1231AngularDemo.Friendships.Dto
{
    public class CreateFriendshipWithDifferentTenantInput
    {
        [Required(AllowEmptyStrings = true)]
        public string TenancyName { get; set; }

        public string UserName { get; set; }
    }
}