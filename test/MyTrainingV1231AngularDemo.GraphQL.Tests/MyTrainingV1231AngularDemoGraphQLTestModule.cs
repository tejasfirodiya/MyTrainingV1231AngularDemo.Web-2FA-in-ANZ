using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MyTrainingV1231AngularDemo.Configure;
using MyTrainingV1231AngularDemo.Startup;
using MyTrainingV1231AngularDemo.Test.Base;

namespace MyTrainingV1231AngularDemo.GraphQL.Tests
{
    [DependsOn(
        typeof(MyTrainingV1231AngularDemoGraphQLModule),
        typeof(MyTrainingV1231AngularDemoTestBaseModule))]
    public class MyTrainingV1231AngularDemoGraphQLTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            IServiceCollection services = new ServiceCollection();
            
            services.AddAndConfigureGraphQL();

            WindsorRegistrationHelper.CreateServiceProvider(IocManager.IocContainer, services);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MyTrainingV1231AngularDemoGraphQLTestModule).GetAssembly());
        }
    }
}