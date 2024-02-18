using MyTrainingV1231AngularDemo.Dto;

namespace MyTrainingV1231AngularDemo.Organizations.Dto
{
    public class FindOrganizationUnitRolesInput : PagedAndFilteredInputDto
    {
        public long OrganizationUnitId { get; set; }
    }
}