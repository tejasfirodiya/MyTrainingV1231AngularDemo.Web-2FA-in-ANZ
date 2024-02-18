using Microsoft.AspNetCore.Mvc;
using MyTrainingV1231AngularDemo.Web.Controllers;

namespace MyTrainingV1231AngularDemo.Web.Public.Controllers
{
    public class AboutController : MyTrainingV1231AngularDemoControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}