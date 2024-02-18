using MyTrainingV1231AngularDemo.Dto;

namespace MyTrainingV1231AngularDemo.Common.Dto
{
    public class FindUsersInput : PagedAndFilteredInputDto
    {
        public int? TenantId { get; set; }

        public bool ExcludeCurrentUser { get; set; }
    }
}