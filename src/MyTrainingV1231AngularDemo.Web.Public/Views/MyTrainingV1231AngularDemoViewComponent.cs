using Abp.AspNetCore.Mvc.ViewComponents;

namespace MyTrainingV1231AngularDemo.Web.Public.Views
{
    public abstract class MyTrainingV1231AngularDemoViewComponent : AbpViewComponent
    {
        protected MyTrainingV1231AngularDemoViewComponent()
        {
            LocalizationSourceName = MyTrainingV1231AngularDemoConsts.LocalizationSourceName;
        }
    }
}