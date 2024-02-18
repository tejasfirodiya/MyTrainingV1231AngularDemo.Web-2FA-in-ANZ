using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MyTrainingV1231AngularDemo.Configuration;
using MyTrainingV1231AngularDemo.Web;

namespace MyTrainingV1231AngularDemo.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class MyTrainingV1231AngularDemoDbContextFactory : IDesignTimeDbContextFactory<MyTrainingV1231AngularDemoDbContext>
    {
        public MyTrainingV1231AngularDemoDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<MyTrainingV1231AngularDemoDbContext>();

            /*
             You can provide an environmentName parameter to the AppConfigurations.Get method. 
             In this case, AppConfigurations will try to read appsettings.{environmentName}.json.
             Use Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") method or from string[] args to get environment if necessary.
             https://docs.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli#args
             */
            var configuration = AppConfigurations.Get(
                WebContentDirectoryFinder.CalculateContentRootFolder(),
                addUserSecrets: true
            );

            MyTrainingV1231AngularDemoDbContextConfigurer.Configure(builder, configuration.GetConnectionString(MyTrainingV1231AngularDemoConsts.ConnectionStringName));

            return new MyTrainingV1231AngularDemoDbContext(builder.Options);
        }
    }
}
