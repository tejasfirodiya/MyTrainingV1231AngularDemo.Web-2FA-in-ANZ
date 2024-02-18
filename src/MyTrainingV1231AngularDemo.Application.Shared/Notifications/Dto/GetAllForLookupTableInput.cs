using Abp.Application.Services.Dto;

namespace MyTrainingV1231AngularDemo.Notifications.Dto
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}