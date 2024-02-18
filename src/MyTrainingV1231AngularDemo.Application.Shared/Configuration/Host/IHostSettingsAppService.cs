using System.Threading.Tasks;
using Abp.Application.Services;
using MyTrainingV1231AngularDemo.Configuration.Host.Dto;

namespace MyTrainingV1231AngularDemo.Configuration.Host
{
    public interface IHostSettingsAppService : IApplicationService
    {
        Task<HostSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(HostSettingsEditDto input);

        Task SendTestEmail(SendTestEmailInput input);
    }
}
