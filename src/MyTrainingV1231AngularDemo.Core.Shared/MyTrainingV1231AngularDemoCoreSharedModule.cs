using Abp.Modules;
using Abp.Reflection.Extensions;

namespace MyTrainingV1231AngularDemo
{
    public class MyTrainingV1231AngularDemoCoreSharedModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MyTrainingV1231AngularDemoCoreSharedModule).GetAssembly());
        }
    }
}