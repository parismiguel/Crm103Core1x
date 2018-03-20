using Abp.AspNetCore.Mvc.Authorization;
using AbpCompanyName.AbpProjectName.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace AbpCompanyName.AbpProjectName.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : AbpProjectNameControllerBase
    {
        public static IHostingEnvironment _wwwRoot;

        public HomeController(IHostingEnvironment environment)
        {
            _wwwRoot = environment;
        }

        public ActionResult Index()
        {
            return View();
        }
	}
}