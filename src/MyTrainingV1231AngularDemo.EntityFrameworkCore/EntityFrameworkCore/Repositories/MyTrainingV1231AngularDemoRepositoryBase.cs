using Abp.Domain.Entities;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Repositories;

namespace MyTrainingV1231AngularDemo.EntityFrameworkCore.Repositories
{
    /// <summary>
    /// Base class for custom repositories of the application.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
    public abstract class MyTrainingV1231AngularDemoRepositoryBase<TEntity, TPrimaryKey> : EfCoreRepositoryBase<MyTrainingV1231AngularDemoDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected MyTrainingV1231AngularDemoRepositoryBase(IDbContextProvider<MyTrainingV1231AngularDemoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add your common methods for all repositories
    }

    /// <summary>
    /// Base class for custom repositories of the application.
    /// This is a shortcut of <see cref="MyTrainingV1231AngularDemoRepositoryBase{TEntity,TPrimaryKey}"/> for <see cref="int"/> primary key.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public abstract class MyTrainingV1231AngularDemoRepositoryBase<TEntity> : MyTrainingV1231AngularDemoRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected MyTrainingV1231AngularDemoRepositoryBase(IDbContextProvider<MyTrainingV1231AngularDemoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)!!!
    }
}
