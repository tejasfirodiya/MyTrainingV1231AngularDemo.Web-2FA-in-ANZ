using Abp.AutoMapper;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Reflection.Extensions;
using MyTrainingV1231AngularDemo.ApiClient;
using MyTrainingV1231AngularDemo.Mobile.MAUI.Core.ApiClient;

namespace MyTrainingV1231AngularDemo
{
    [DependsOn(typeof(MyTrainingV1231AngularDemoClientModule), typeof(AbpAutoMapperModule))]

    public class MyTrainingV1231AngularDemoMobileMAUIModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Localization.IsEnabled = false;
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;

            Configuration.ReplaceService<IApplicationContext, MAUIApplicationContext>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MyTrainingV1231AngularDemoMobileMAUIModule).GetAssembly());
        }
    }
}