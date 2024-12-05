using System.Web;
using System.Web.Mvc;

public class AuthorizeSessionAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var session = filterContext.HttpContext.Session;

        // Kiểm tra nếu session "User" không tồn tại hoặc không hợp lệ
        if (session["quanly"] == null)
        {
            // Redirect về trang đăng nhập
            filterContext.Result = new RedirectToRouteResult(
                new System.Web.Routing.RouteValueDictionary
                {
                    { "controller", "DangNhapAdmin" },
                    { "action", "Login_Admin" },
                    { "area", "" } // Xác định area nếu cần
                });
        }
        base.OnActionExecuting(filterContext);
    }
}
