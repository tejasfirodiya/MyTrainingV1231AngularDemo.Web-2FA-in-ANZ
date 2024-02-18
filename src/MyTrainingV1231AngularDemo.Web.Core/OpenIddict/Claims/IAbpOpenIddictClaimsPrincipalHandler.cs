using System.Threading.Tasks;

namespace MyTrainingV1231AngularDemo.Web.OpenIddict.Claims
{
    public interface IAbpOpenIddictClaimsPrincipalHandler
    {
        Task HandleAsync(AbpOpenIddictClaimsPrincipalHandlerContext context);
    }
}