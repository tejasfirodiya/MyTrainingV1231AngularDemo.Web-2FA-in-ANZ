using Abp.Collections;

namespace MyTrainingV1231AngularDemo.Web.OpenIddict.Claims;

public class AbpOpenIddictClaimsPrincipalOptions
{
    public ITypeList<IAbpOpenIddictClaimsPrincipalHandler> ClaimsPrincipalHandlers { get; }

    public AbpOpenIddictClaimsPrincipalOptions()
    {
        ClaimsPrincipalHandlers = new TypeList<IAbpOpenIddictClaimsPrincipalHandler>();
    }
}