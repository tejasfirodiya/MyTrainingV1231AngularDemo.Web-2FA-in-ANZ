using System.Threading.Tasks;
using Abp.Application.Services;
using MyTrainingV1231AngularDemo.Sessions.Dto;

namespace MyTrainingV1231AngularDemo.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();

        Task<UpdateUserSignInTokenOutput> UpdateUserSignInToken();
    }
}
