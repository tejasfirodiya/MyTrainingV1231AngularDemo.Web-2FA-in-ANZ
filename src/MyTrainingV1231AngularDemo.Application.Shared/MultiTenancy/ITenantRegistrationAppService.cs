using System.Threading.Tasks;
using Abp.Application.Services;
using MyTrainingV1231AngularDemo.Editions.Dto;
using MyTrainingV1231AngularDemo.MultiTenancy.Dto;

namespace MyTrainingV1231AngularDemo.MultiTenancy
{
    public interface ITenantRegistrationAppService: IApplicationService
    {
        Task<RegisterTenantOutput> RegisterTenant(RegisterTenantInput input);

        Task<EditionsSelectOutput> GetEditionsForSelect();

        Task<EditionSelectDto> GetEdition(int editionId);
    }
}