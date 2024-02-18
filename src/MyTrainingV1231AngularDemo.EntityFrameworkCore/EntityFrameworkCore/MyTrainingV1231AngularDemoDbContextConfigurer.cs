using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace MyTrainingV1231AngularDemo.EntityFrameworkCore
{
    public static class MyTrainingV1231AngularDemoDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<MyTrainingV1231AngularDemoDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<MyTrainingV1231AngularDemoDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}