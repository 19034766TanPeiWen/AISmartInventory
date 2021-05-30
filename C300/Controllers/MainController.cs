using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using C300.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace C300.Controllers
{
    public class MainController : Controller
    {
        private IWebHostEnvironment _env;

        #region trends
        [HttpGet]
        public IActionResult trends(int id)
        {
            string productSql = @"SELECT * FROM Product
                                 WHERE Product_ID = {0}";
            List<Product> lstProduct = DBUtl.GetList<Product>(productSql, id);

            // If the record is found, pass the model to the View
            if (lstProduct.Count == 1)
            {
                ViewData["Category"] = GetListProduct();
                return View(lstProduct[0]);
            }
            else
            // Otherwise redirect to the movie list page
            {
                TempData["Message"] = "Product not found.";
                TempData["MsgType"] = "warning";
                return RedirectToAction("ListProduct");
            }
        }
        [HttpPost]
        public IActionResult trendsPost()
        {

            IFormCollection form = HttpContext.Request.Form;
            string trends = form["Trends"].ToString();
            if (ValidUtl.CheckIfEmpty(trends))
            {
                ViewData["Message"] = "Please enter all fields";
                ViewData["MsgType"] = "warning";
                return View("trends");
            }

            return RedirectToAction("Chart");
        }
        #endregion
        #region Chart
        public IActionResult Bar()
        {
            ViewData["Chart"] = "bar";
            ViewData["ShowLegend"] = "false";
            return View("Chart");
        }

        public IActionResult Pie()
        {
            ViewData["Chart"] = "pie";
            ViewData["ShowLegend"] = "true";
            return View("Chart");
        }

        public IActionResult Line()
        {
            ViewData["Chart"] = "line";
            ViewData["ShowLegend"] = "false";
            return View("Chart");
        }

        public IActionResult Chart(int id)
        { 
            string sql = @"SELECT Product_Name AS Product, Quantity 
                         FROM Product
                         WHERE Product_ID = {0}";
            List<Product> list = DBUtl.GetList<Product>(sql, id);

            if (list != null)
            {
                Product pdt = list[0];
                int[] data = new int[]{ pdt.ReorderQty };
                ViewData["Data"] = data;
            }
            else
            {
                TempData["Message"] = "Record does not exist";
                TempData["MsgType"] = "warning";
                return RedirectToAction("trends");
            }

            string[] colors = new[] { "cyan", "lightgreen", "yellow", "lightgrey", "pink", "grey", "blue", "green" };
            string[] grades = new[] { "Anger", "Contempt", "Disgust", "Fear", "Happiness", "Neutral", "Sadness", "Surprise" };
            ViewData["Chart"] = "pie";
            ViewData["Title"] = "Monthly Sales Summary";
            ViewData["ShowLegend"] = "true";
            ViewData["Legend"] = "Cadets";
            ViewData["Colors"] = colors;
            ViewData["Labels"] = grades;

            return View("Chart");
        }
        #endregion
        public MainController(IWebHostEnvironment environment)
        {
            _env = environment;
        }
        private SelectList GetListProduct()
        {
            // Get a list of all genres from the database
            var catSql = DBUtl.GetList("SELECT Category FROM Movie");
            SelectList lstProduct = new SelectList(catSql, "Category");
            return lstProduct;
        }
    }
}
