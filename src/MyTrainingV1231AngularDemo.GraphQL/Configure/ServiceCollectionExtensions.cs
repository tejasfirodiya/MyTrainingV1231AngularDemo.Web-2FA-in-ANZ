using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using MyTrainingV1231AngularDemo.Debugging;
using MyTrainingV1231AngularDemo.Schemas;

namespace MyTrainingV1231AngularDemo.Configure
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAndConfigureGraphQL(this IServiceCollection services)
        {
            services
                .AddGraphQL(x => { x.EnableMetrics = DebugHelper.IsDebug; })
                .AddNewtonsoftJson(deserializerSettings => { }, serializerSettings => { }) // For everything else
                .AddErrorInfoProvider(opt => opt.ExposeExceptionStackTrace = DebugHelper.IsDebug)
                .AddGraphTypes(ServiceLifetime.Scoped)
                .AddUserContextBuilder(httpContext => new Dictionary<string, object>
                {
                    {"user", httpContext.User}
                })
                .AddNewtonsoftJson(deserializerSettings => { }, serializerSettings => { }) // For everything else
                .AddDataLoader();

            AllowSynchronousIo(services);
        }

        //https://github.com/graphql-dotnet/graphql-dotnet/issues/1326
        private static void AllowSynchronousIo(IServiceCollection services)
        {
            // kestrel
            services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });

            // IIS
            services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });
        }
    }
}
