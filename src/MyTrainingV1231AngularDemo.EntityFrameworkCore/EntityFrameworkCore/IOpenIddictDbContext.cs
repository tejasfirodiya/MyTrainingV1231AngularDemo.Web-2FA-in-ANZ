using Microsoft.EntityFrameworkCore;
using MyTrainingV1231AngularDemo.OpenIddict.Applications;
using MyTrainingV1231AngularDemo.OpenIddict.Authorizations;
using MyTrainingV1231AngularDemo.OpenIddict.Scopes;
using MyTrainingV1231AngularDemo.OpenIddict.Tokens;

namespace MyTrainingV1231AngularDemo.EntityFrameworkCore
{
    public interface IOpenIddictDbContext
    {
        DbSet<OpenIddictApplication> Applications { get; }

        DbSet<OpenIddictAuthorization> Authorizations { get; }

        DbSet<OpenIddictScope> Scopes { get; }

        DbSet<OpenIddictToken> Tokens { get; }
    }

}