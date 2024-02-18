using System;
using System.Threading;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MyTrainingV1231AngularDemo.EntityFrameworkCore;

namespace MyTrainingV1231AngularDemo.HealthChecks
{
    public class MyTrainingV1231AngularDemoDbContextUsersHealthCheck : IHealthCheck
    {
        private readonly IDbContextProvider<MyTrainingV1231AngularDemoDbContext> _dbContextProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public MyTrainingV1231AngularDemoDbContextUsersHealthCheck(
            IDbContextProvider<MyTrainingV1231AngularDemoDbContext> dbContextProvider,
            IUnitOfWorkManager unitOfWorkManager
            )
        {
            _dbContextProvider = dbContextProvider;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                using (var uow = _unitOfWorkManager.Begin())
                {
                    // Switching to host is necessary for single tenant mode.
                    using (_unitOfWorkManager.Current.SetTenantId(null))
                    {
                        var dbContext = await _dbContextProvider.GetDbContextAsync();
                        if (!await dbContext.Database.CanConnectAsync(cancellationToken))
                        {
                            return HealthCheckResult.Unhealthy(
                                "MyTrainingV1231AngularDemoDbContext could not connect to database"
                            );
                        }

                        var user = await dbContext.Users.AnyAsync(cancellationToken);
                        await uow.CompleteAsync();

                        if (user)
                        {
                            return HealthCheckResult.Healthy("MyTrainingV1231AngularDemoDbContext connected to database and checked whether user added");
                        }

                        return HealthCheckResult.Unhealthy("MyTrainingV1231AngularDemoDbContext connected to database but there is no user.");

                    }
                }
            }
            catch (Exception e)
            {
                return HealthCheckResult.Unhealthy("MyTrainingV1231AngularDemoDbContext could not connect to database.", e);
            }
        }
    }
}
