using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication.Template
{
    public partial class LogoutPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Session'ı sonlandır
            Session.Clear();
            Session.Abandon();

            // Çerezleri temizlemek için
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));

            // LoginPage'e yönlendir
            Response.Redirect("LoginPage.aspx");
        }
    }
}