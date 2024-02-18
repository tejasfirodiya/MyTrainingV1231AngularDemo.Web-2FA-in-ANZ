using Abp.Dependency;
using GraphQL.Types;
using GraphQL.Utilities;
using MyTrainingV1231AngularDemo.Queries.Container;
using System;

namespace MyTrainingV1231AngularDemo.Schemas
{
    public class MainSchema : Schema, ITransientDependency
    {
        public MainSchema(IServiceProvider provider) :
            base(provider)
        {
            Query = provider.GetRequiredService<QueryContainer>();
        }
    }
}