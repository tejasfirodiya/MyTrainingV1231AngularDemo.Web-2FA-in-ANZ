using MyTrainingV1231AngularDemo.Dto;

namespace MyTrainingV1231AngularDemo.WebHooks.Dto
{
    public class GetAllSendAttemptsInput : PagedInputDto
    {
        public string SubscriptionId { get; set; }
    }
}
