using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using MyTrainingV1231AngularDemo.Authorization;

namespace MyTrainingV1231AngularDemo
{
    /// <summary>
    /// Application layer module of the application.
    /// </summary>
    [DependsOn(
        typeof(MyTrainingV1231AngularDemoApplicationSharedModule),
        typeof(MyTrainingV1231AngularDemoCoreModule)
        )]
    public class MyTrainingV1231AngularDemoApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Adding authorization providers
            Configuration.Authorization.Providers.Add<AppAuthorizationProvider>();

            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MyTrainingV1231AngularDemoApplicationModule).GetAssembly());
        }
    }
}