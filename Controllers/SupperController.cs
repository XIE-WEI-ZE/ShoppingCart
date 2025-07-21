using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using prjECommerceDemo.ViewModel;

namespace prjECommerceDemo.Controllers
{
    public class SupperController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // 若尚未登入，導回首頁登入頁
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_LOGINED_USER))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "Login"
                }));
            }
        }
    }
}
