using System.Web;
using System.Web.Mvc;
using NhaXe_QuocVuong.Models;

public class CheckSessionRoleAttribute : ActionFilterAttribute
{
    private readonly string _requiredRole;

    public CheckSessionRoleAttribute(string requiredRole)
    {
        _requiredRole = requiredRole;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var session = filterContext.HttpContext.Session;

        // Kiểm tra session "role" có tồn tại và có quyền phù hợp không
        if (session["quanly"] == null )
        {

            // Chuyển hướng về trang đăng nhập nếu không đủ quyền
            filterContext.Result = new RedirectToRouteResult(
                new System.Web.Routing.RouteValueDictionary
                {
                    { "controller", "DangNhapAdmin" },
                    { "action", "Login_Admin" },
                    { "area", "" } // Thay "" bằng area nếu cần
                });
        }
        var tenQl = session["quanly"] as userAccount;
        if  (tenQl.NHANVIEN.LOAI_NV != "QUAN_LY") {
            filterContext.Result = new ContentResult
            {
                Content = "<h3>Bạn không có quyền truy cập vào chức năng này.</h3>",
                ContentType = "text/html"
            };
        } 
        base.OnActionExecuting(filterContext);
    }
}
