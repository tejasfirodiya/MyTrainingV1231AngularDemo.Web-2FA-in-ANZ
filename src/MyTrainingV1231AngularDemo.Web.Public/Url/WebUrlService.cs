using Abp.Dependency;
using MyTrainingV1231AngularDemo.Configuration;
using MyTrainingV1231AngularDemo.Url;
using MyTrainingV1231AngularDemo.Web.Url;

namespace MyTrainingV1231AngularDemo.Web.Public.Url
{
    public class WebUrlService : WebUrlServiceBase, IWebUrlService, ITransientDependency
    {
        public WebUrlService(
            IAppConfigurationAccessor appConfigurationAccessor) :
            base(appConfigurationAccessor)
        {
        }

        public override string WebSiteRootAddressFormatKey => "App:WebSiteRootAddress";

        public override string ServerRootAddressFormatKey => "App:AdminWebSiteRootAddress";
    }
}