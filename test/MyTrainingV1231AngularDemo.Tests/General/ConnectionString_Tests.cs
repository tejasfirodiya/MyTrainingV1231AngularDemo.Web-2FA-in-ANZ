using System.Data.SqlClient;
using Shouldly;
using Xunit;

namespace MyTrainingV1231AngularDemo.Tests.General
{
    // ReSharper disable once InconsistentNaming
    public class ConnectionString_Tests
    {
        [Fact]
        public void SqlConnectionStringBuilder_Test()
        {
            var csb = new SqlConnectionStringBuilder("Server=localhost; Database=MyTrainingV1231AngularDemo; Trusted_Connection=True;");
            csb["Database"].ShouldBe("MyTrainingV1231AngularDemo");
        }
    }
}
