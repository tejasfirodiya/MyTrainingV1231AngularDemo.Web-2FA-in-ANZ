using Abp.AspNetCore.Mvc.Views;

namespace MyTrainingV1231AngularDemo.Web.Views
{
    public abstract class MyTrainingV1231AngularDemoRazorPage<TModel> : AbpRazorPage<TModel>
    {
        protected MyTrainingV1231AngularDemoRazorPage()
        {
            LocalizationSourceName = MyTrainingV1231AngularDemoConsts.LocalizationSourceName;
        }
    }
}
