using AbpCompanyName.AbpProjectName.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace AbpCompanyName.AbpProjectName.Web.Mvc.Controllers
{
    public class ToolsController : AbpProjectNameControllerBase
    {
        public ActionResult GooglePhoneSearch()
        {
            return View();
        }

        public ActionResult CustomPhoneSearch()
        {
            return View();
        }
    }
}