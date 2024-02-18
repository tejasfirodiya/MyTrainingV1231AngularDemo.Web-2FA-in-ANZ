using Abp.Modules;
using Abp.Reflection.Extensions;

namespace MyTrainingV1231AngularDemo
{
    [DependsOn(typeof(MyTrainingV1231AngularDemoCoreSharedModule))]
    public class MyTrainingV1231AngularDemoApplicationSharedModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MyTrainingV1231AngularDemoApplicationSharedModule).GetAssembly());
        }
    }
}