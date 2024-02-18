using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MyTrainingV1231AngularDemo.EntityFrameworkCore;

namespace MyTrainingV1231AngularDemo.HealthChecks
{
    public class MyTrainingV1231AngularDemoDbContextHealthCheck : IHealthCheck
    {
        private readonly DatabaseCheckHelper _checkHelper;

        public MyTrainingV1231AngularDemoDbContextHealthCheck(DatabaseCheckHelper checkHelper)
        {
            _checkHelper = checkHelper;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            if (_checkHelper.Exist("db"))
            {
                return Task.FromResult(HealthCheckResult.Healthy("MyTrainingV1231AngularDemoDbContext connected to database."));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("MyTrainingV1231AngularDemoDbContext could not connect to database"));
        }
    }
}
