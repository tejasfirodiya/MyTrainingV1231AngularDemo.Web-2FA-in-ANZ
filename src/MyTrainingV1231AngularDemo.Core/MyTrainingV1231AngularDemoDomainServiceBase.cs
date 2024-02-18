using Abp.Domain.Services;

namespace MyTrainingV1231AngularDemo
{
    public abstract class MyTrainingV1231AngularDemoDomainServiceBase : DomainService
    {
        /* Add your common members for all your domain services. */

        protected MyTrainingV1231AngularDemoDomainServiceBase()
        {
            LocalizationSourceName = MyTrainingV1231AngularDemoConsts.LocalizationSourceName;
        }
    }
}
