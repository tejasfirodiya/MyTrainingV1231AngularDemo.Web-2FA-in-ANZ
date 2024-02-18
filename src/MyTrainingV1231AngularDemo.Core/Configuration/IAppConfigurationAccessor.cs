using Microsoft.Extensions.Configuration;

namespace MyTrainingV1231AngularDemo.Configuration
{
    public interface IAppConfigurationAccessor
    {
        IConfigurationRoot Configuration { get; }
    }
}
