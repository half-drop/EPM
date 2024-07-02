using EPM.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EPM.Controllers
{
    public class HomeController : Controller
    {
        private EPMEntities db = new EPMEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return Content("<script>alert('账号密码不能为空');location.href='Login';</script>", "text/html");
            }

            var user = db.Employees.Where(u => u.Username == username && u.Password == password).SingleOrDefault();

            if (user == null)
            {
                return Content("<script>alert('输入有误');location.href='Login';</script>", "text/html");
            }
            else
            {
                FormsAuthentication.SetAuthCookie(username, false);
                TempData["Username"] = username;
                TempData["UserRole"] = user.Role;
                return RedirectToAction("Dashboard", "Employees");
            }
        }
    }
}