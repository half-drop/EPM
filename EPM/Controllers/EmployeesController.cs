using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using EPM.Models;

namespace EPM.Controllers
{
    public class EmployeesController : Controller
    {
        private EPMEntities db = new EPMEntities();

        // GET: Employees
        public ActionResult Index()
        {
            return View(db.Employees.ToList());
        }

        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employees employees = db.Employees.Find(id);
            if (employees == null)
            {
                return HttpNotFound();
            }
            return View(employees);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性。有关
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeID,Name,Username,Password,Role,Contact,CurrentDepartment,Salary")] Employees employees)
        {
            if (ModelState.IsValid)
            {
                db.Employees.Add(employees);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(employees);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employees employees = db.Employees.Find(id);
            if (employees == null)
            {
                return HttpNotFound();
            }
            return View(employees);
        }

        // POST: Employees/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性。有关
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,Name,Username,Password,Role,Contact,CurrentDepartment,Salary")] Employees employees)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employees).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employees);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employees employees = db.Employees.Find(id);
            if (employees == null)
            {
                return HttpNotFound();
            }
            return View(employees);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employees employees = db.Employees.Find(id);
            db.Employees.Remove(employees);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Dashboard()
        {
            var username = User.Identity.Name;
            var employee = db.Employees.FirstOrDefault(u => u.Username == username);
            if (employee == null)
            {
                return HttpNotFound("用户未找到");
            }

            ViewBag.Role = employee.Role;

            return View(employee);
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: Employees/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 获取当前登录的用户ID
            var username = User.Identity.Name;
            var user = db.Employees.FirstOrDefault(e => e.Username == username);
            if (user == null)
            {
                return HttpNotFound("未找到用户。");
            }

            if (!IsPasswordCorrect(user, model.OldPassword))
            {
                ViewBag.PasswordError = "当前密码不正确。";
                return View(model);
            }

            user.Password = EncryptPassword(model.NewPassword);
            db.SaveChanges();
            ViewBag.PasswordFinish = "密码已经修改";

            return RedirectToAction("Dashboard");
        }

        private bool IsPasswordCorrect(Employees user, string password)
        {
            return user.Password == password;
        }

        private string EncryptPassword(string password)
        {
            return password;
        }

        private string GenerateCsvFromEmployees(List<Employees> employees)
        {
            var sb = new StringBuilder();
            sb.AppendLine("员工编号,姓名,职称,工资,权限,部门");
            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.EmployeeID},{emp.Name},{emp.Contact},{emp.Salary},{emp.Role},{emp.CurrentDepartment}");
            }
            return sb.ToString();
        }

        public ActionResult Export()
        {
            var employees = db.Employees.ToList();
            var csv = GenerateCsvFromEmployees(employees);
            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv)).ToArray();
            return File(bytes, "text/csv", "员工信息表.csv");
        }

        public ActionResult Backup()
        {
            string databaseName = "EPM";
            string backupFileName = $"F:\\2023-2024(2)\\软工实习\\项目文件\\SQL\\EPM.bak";

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnection"].ConnectionString))
            {
                var query = $@"
BACKUP DATABASE [{databaseName}]
TO DISK = '{backupFileName}'
WITH FORMAT, MEDIANAME = 'SQLServerBackups',
NAME = 'Full Backup of {databaseName}';";

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            string script = "<script type='text/javascript'>" +
                    "alert('数据库备份成功！');" +
                    "setTimeout(function(){" +
                    "window.location.href='" + Url.Action("Dashboard", "Employees") + "';" +
                    "}, 0);" +
                    "</script>";

            return Content(script, "text/html");
        }

        public ActionResult Restore()
        {
            string databaseName = "EPM";
            string backupFileName = "F:\\2023-2024(2)\\软工实习\\项目文件\\SQL\\EPM.bak";

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnection"].ConnectionString))
            {
                var query = $@"
USE master;
ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
RESTORE DATABASE [{databaseName}] FROM DISK = '{backupFileName}' WITH REPLACE;
ALTER DATABASE [{databaseName}] SET MULTI_USER;";

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            string script = "<script type='text/javascript'>" +
                    "alert('数据库还原成功！');" +
                    "setTimeout(function(){" +
                    "window.location.href='" + Url.Action("Dashboard", "Employees") + "';" +
                    "}, 0);" +
                    "</script>";

            return Content(script, "text/html");
        }
    }
}