using Abp.Application.Services.Dto;

namespace MyTrainingV1231AngularDemo.Authorization.Users.Dto
{
    public interface IGetLoginAttemptsInput: ISortedResultRequest
    {
        string Filter { get; set; }
    }
}