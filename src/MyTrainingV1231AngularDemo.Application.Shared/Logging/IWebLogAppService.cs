using Abp.Application.Services;
using MyTrainingV1231AngularDemo.Dto;
using MyTrainingV1231AngularDemo.Logging.Dto;

namespace MyTrainingV1231AngularDemo.Logging
{
    public interface IWebLogAppService : IApplicationService
    {
        GetLatestWebLogsOutput GetLatestWebLogs();

        FileDto DownloadWebLogs();
    }
}
