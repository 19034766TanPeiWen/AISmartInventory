using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Security.Claims;
using C300.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace C300.Controllers
{

   public class AccountController : Controller
    {
      private const string LOGIN_SQL =
         @"SELECT * FROM Employee
            WHERE EmployeeNo = '{0}' AND CompanyId = {1}";

      //private const string ROLE_COL = "UserRole";
      private const string NAME_COL = "LastName";

      private const string REDIRECT_CNTR = "Product";
      private const string REDIRECT_ACTN = "About";

      private const string LOGIN_VIEW = "UserLogin";

      [AllowAnonymous]
      public IActionResult UserLogin(string returnUrl = null)
      {
         ViewData["Company"] = GetListCom();
         TempData["ReturnUrl"] = returnUrl;
         return View(LOGIN_VIEW);
      }

      [AllowAnonymous]
      [HttpPost]
      public IActionResult UserLogin(UserLogin user)
      {
         if (!AuthenticateUser(user.EmployeeNo, user.CompanyId, out ClaimsPrincipal principal))
         {
            ViewData["Message"] = "Invalid Account";
            ViewData["MsgType"] = "warning";
            return View(LOGIN_VIEW);
         }
         else
         {
            HttpContext.SignInAsync(
               CookieAuthenticationDefaults.AuthenticationScheme,
               principal,
           new AuthenticationProperties
           {
              IsPersistent = user.RememberMe
           });

            return RedirectToAction(REDIRECT_ACTN, REDIRECT_CNTR);
         }
      }

      [Authorize]
      public IActionResult Logoff(string returnUrl = null)
      {
         HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
         if (Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
         return RedirectToAction(REDIRECT_ACTN, REDIRECT_CNTR);
      }

      [AllowAnonymous]
      public IActionResult Forbidden()
      {
         return View();
      }

      [Authorize]
      public IActionResult Users()
      {
         List<UserDetails> list = DBUtl.GetList<UserDetails>(@"SELECT Employee.Id, EmployeeNo, LastName, GivenName,
                                                                OtherNames, Dob, TradingAs
                                                                FROM Employee, Company
                                                                WHERE Employee.CompanyId = Company.Id");
         return View(list);
      }

      [Authorize]
      public IActionResult Delete(int id)
      {
         string delete = "DELETE FROM Employee WHERE Id={0}";
         int res = DBUtl.ExecSQL(delete, id);
         if (res == 1)
         {
            TempData["Message"] = "User Record Deleted";
            TempData["MsgType"] = "success";
         }
         else
         {
            TempData["Message"] = DBUtl.DB_Message;
            TempData["MsgType"] = "danger";
         }

         return RedirectToAction("Users");
      }

      [AllowAnonymous]
      public IActionResult UserRegister()
      {
            ViewData["Company"] = GetListCom();
            return View("UserRegister");
      }

      [AllowAnonymous]
      [HttpPost]
      public IActionResult UserRegister(UserDetails usr)
      {
         ModelState.Remove("OtherNames");
         if (!ModelState.IsValid)
         {
            ViewData["Company"] = GetListCom();
            ViewData["Message"] = "Invalid Input";
            ViewData["MsgType"] = "warning";
            return View("UserRegister", usr);
         }
         else
         {
            string insert =
               @"INSERT INTO Employee(EmployeeNo, LastName, GivenName, OtherNames, Dob, CompanyId) 
                VALUES('{0}', '{1}', '{2}', '{3}','{4:yyyy-MM-dd}',{5})";
                if (DBUtl.ExecSQL(insert, usr.EmployeeNo, usr.LastName, usr.GivenName, usr.OtherNames,usr.Dob,usr.CompanyId) == 1)
            {
                  ViewData["Message"] = "Employee Successfully Registered";
                  ViewData["MsgType"] = "success"; 
            }
            else
            {
               ViewData["Message"] = DBUtl.DB_Message;
               ViewData["MsgType"] = "danger";
            }
            return View("User");
         }
      }

      [AllowAnonymous]
      public IActionResult VerifyUserID(string EmpId, int id)
      {
         string select = $"SELECT * FROM Employee WHERE Employee='{EmpId}' AND CompanyId={id}";
         if (DBUtl.GetTable(select).Rows.Count > 0)
         {
            return Json($"[{EmpId}] already in use");
         }
         return Json(true);
      }

      private bool AuthenticateUser(string empid, int id, out ClaimsPrincipal principal)
      {
         principal = null;

         DataTable ds = DBUtl.GetTable(LOGIN_SQL, empid, id);
         if (ds.Rows.Count == 1)
         {
            principal =
               new ClaimsPrincipal(
                  new ClaimsIdentity(
                     new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier, empid),
                        new Claim(ClaimTypes.Name, ds.Rows[0][NAME_COL].ToString()),
                        //new Claim(ClaimTypes.Role, ds.Rows[0][ROLE_COL].ToString())
                     }, "Basic"
                  )
               );
            return true;
         }
         return false;
      }
        private SelectList GetListCom()
        {

            var comSql = DBUtl.GetList("SELECT Id, TradingAs FROM Company");
            SelectList lstType = new SelectList(comSql, "Id", "TradingAs");
            return lstType;
        }
    }
}