using Abp.Dependency;

namespace MyTrainingV1231AngularDemo.Web.Xss
{
    public interface IHtmlSanitizer: ITransientDependency
    {
        string Sanitize(string html);
    }
}