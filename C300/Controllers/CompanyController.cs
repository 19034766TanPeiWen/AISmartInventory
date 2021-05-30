using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using C300.Models;
using System.Security.Claims;

namespace C300.Controllers
{
    public class CompanyController : Controller
    {
        [Authorize]
        public IActionResult ListCompany()
        {
            List<Company> company = DBUtl.GetList<Company>(@"SELECT * FROM Company");
            return View(company);

        }
        [Authorize]
        public IActionResult AddCompany()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public IActionResult AddCompany(Company newCom)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("AddCompany");
            }
            else
            {
                string insert =
                   @"INSERT INTO Company(TradingAs, UEN, IncorporationDate, RegisteredOffice) 
                 VALUES('{0}', '{1}', '{2}', '{3}')";
                int result = DBUtl.ExecSQL(insert, newCom.TradingAs, newCom.UEN, newCom.IncorporationDate, newCom.RegisteredOffice);  

                if (result == 1)
                {
                    TempData["Message"] = "Company Created";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
                return RedirectToAction("ListCompany");
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult EditCompany(int id)
        {
            string productSql = @"SELECT * FROM Company
                    WHERE Id= {0}";
            List<Company> lstCompany = DBUtl.GetList<Company>(productSql, id);

            if (lstCompany.Count == 1)
            {
                return View(lstCompany[0]);
            }
            else
            {
                TempData["Message"] = "Company not found.";
                TempData["MsgType"] = "warning";
                return RedirectToAction("ListCompany");
            }
        }
        [Authorize]
        [HttpPost]
        public IActionResult EditCompany(Company company)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "danger";
                return View("EditCompany", company);
            }
            else
            {
                string update = @"UPDATE Company 
                              SET TradingAs = '{1}', 
                                  UEN = '{2}', 
                                  IncorporationDate = '{3}', 
                                  RegisteredOffice = '{4}'                                 
                              WHERE Id={0} ";
                if (DBUtl.ExecSQL(update, company.Id, company.TradingAs, company.UEN,
                                  company.IncorporationDate, company.RegisteredOffice)==1)
                {
                    TempData["Message"] = "Company Updated";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
                return RedirectToAction("ListCompany");
            }            
        }
        [Authorize]
        public IActionResult DeleteCompany(int id)
        {
            string select = @"SELECT * FROM Company 
                              WHERE Id={0}";
            DataTable ds = DBUtl.GetTable(select, id);
            if (ds.Rows.Count != 1)
            {
                TempData["Message"] = "Company record no longer exists.";
                TempData["MsgType"] = "warning";
            }
            else
            {
                string delete = "DELETE FROM Company WHERE Id={0}";
                int res = DBUtl.ExecSQL(delete, id);
                if (res == 1)
                {
                    TempData["Message"] = "Company Deleted";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
            }
            return RedirectToAction("ListCompany");
        }
    }
}