using Microsoft.AspNetCore.Antiforgery;

namespace MyTrainingV1231AngularDemo.Web.Controllers
{
    public class AntiForgeryController : MyTrainingV1231AngularDemoControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}
