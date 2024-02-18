using Abp;

namespace MyTrainingV1231AngularDemo
{
    /// <summary>
    /// This class can be used as a base class for services in this application.
    /// It has some useful objects property-injected and has some basic methods most of services may need to.
    /// It's suitable for non domain nor application service classes.
    /// For domain services inherit <see cref="MyTrainingV1231AngularDemoDomainServiceBase"/>.
    /// For application services inherit MyTrainingV1231AngularDemoAppServiceBase.
    /// </summary>
    public abstract class MyTrainingV1231AngularDemoServiceBase : AbpServiceBase
    {
        protected MyTrainingV1231AngularDemoServiceBase()
        {
            LocalizationSourceName = MyTrainingV1231AngularDemoConsts.LocalizationSourceName;
        }
    }
}