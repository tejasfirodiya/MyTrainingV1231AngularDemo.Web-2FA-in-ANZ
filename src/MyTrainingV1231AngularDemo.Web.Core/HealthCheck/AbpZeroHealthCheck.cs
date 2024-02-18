using Microsoft.Extensions.DependencyInjection;
using MyTrainingV1231AngularDemo.HealthChecks;

namespace MyTrainingV1231AngularDemo.Web.HealthCheck
{
    public static class AbpZeroHealthCheck
    {
        public static IHealthChecksBuilder AddAbpZeroHealthCheck(this IServiceCollection services)
        {
            var builder = services.AddHealthChecks();
            builder.AddCheck<MyTrainingV1231AngularDemoDbContextHealthCheck>("Database Connection");
            builder.AddCheck<MyTrainingV1231AngularDemoDbContextUsersHealthCheck>("Database Connection with user check");
            builder.AddCheck<CacheHealthCheck>("Cache");

            // add your custom health checks here
            // builder.AddCheck<MyCustomHealthCheck>("my health check");

            return builder;
        }
    }
}
