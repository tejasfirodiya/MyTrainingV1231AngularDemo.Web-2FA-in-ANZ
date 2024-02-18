using System.Threading.Tasks;
using Abp.Application.Services;
using MyTrainingV1231AngularDemo.Install.Dto;

namespace MyTrainingV1231AngularDemo.Install
{
    public interface IInstallAppService : IApplicationService
    {
        Task Setup(InstallDto input);

        AppSettingsJsonDto GetAppSettingsJson();

        CheckDatabaseOutput CheckDatabase();
    }
}