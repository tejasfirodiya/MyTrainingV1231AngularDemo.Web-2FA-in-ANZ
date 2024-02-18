using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace MyTrainingV1231AngularDemo.Web.Public.Views
{
    public abstract class MyTrainingV1231AngularDemoRazorPage<TModel> : AbpRazorPage<TModel>
    {
        [RazorInject]
        public IAbpSession AbpSession { get; set; }

        protected MyTrainingV1231AngularDemoRazorPage()
        {
            LocalizationSourceName = MyTrainingV1231AngularDemoConsts.LocalizationSourceName;
        }
    }
}
