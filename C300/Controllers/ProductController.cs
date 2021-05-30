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
    public class ProductController : Controller
    {
        [AllowAnonymous]
        public IActionResult About()
        {
            return View();
        }
        [Authorize]
        public IActionResult ListProduct()
        {
            List<Product> product = DBUtl.GetList<Product>(
                  @"SELECT Product.Id, Description, Weight, Width, Height, Depth, Type, CategoryDescription, Isle, Shelf, ReorderQty
                    FROM Product, Package, Category, Location 
                    WHERE Product.PackageId = Package.Id
                    AND Product.CategoryId = Category.Id
                    AND Product.LocationId = Location.Id");
            return View(product);

        }
        [Authorize]
        public IActionResult AddProduct()
        {
            ViewData["Category"] = GetListCat();
            ViewData["Package"] = GetListType();
            return View();
        }
        [Authorize]
        [HttpPost]
        public IActionResult AddProduct(Product newProduct)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Category"] = GetListCat();
                ViewData["Package"] = GetListType();
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("AddProduct");
            }
            else
            {
                string insert =
                   @"INSERT INTO Product(Description, Weight, Width, Height, Depth, PackageId, CategoryId, LocationId, ReorderQty) 
                 VALUES('{0}', {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})";
                int result = DBUtl.ExecSQL(insert, newProduct.Description, newProduct.Weight, newProduct.Width,
                                           newProduct.Height, newProduct.Depth, newProduct.PackageId,
                                           newProduct.CategoryId,newProduct.LocationId,newProduct.ReorderQty);  

                if (result == 1)
                {
                    TempData["Message"] = "Product Created";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
                return RedirectToAction("ListProduct");
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult EditProduct(string name)
        {
            string productSql = @"SELECT * FROM Product
                    WHERE Description = '{0}'";
            List<Product> lstProduct = DBUtl.GetList<Product>(productSql, name);

            if (lstProduct.Count == 1)
            {
                ViewData["Category"] = GetListCat();
                ViewData["ProductType"] = GetListType();
                return View(lstProduct[0]);
            }
            else
            {
                TempData["Message"] = "Product not found.";
                TempData["MsgType"] = "warning";
                return RedirectToAction("ListProduct");
            }
        }
        [Authorize]
        [HttpPost]
        public IActionResult EditProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "danger";
                return View("EditProduct", product);
            }
            else
            {
                string update = @"UPDATE Product 
                              SET Product.Weight={1}, Product.Width={2}, Product.Height={3}, Product.Depth={4}, Package.Type={5},
                                  Category.CategoryDescription = '{6}', Location.Isle = '{7}', Location.Shelf = '{8}', ReorderQty = {9}              
                              WHERE Product.Id={0} 
                              AND Category.Id = Product.CategoryId, 
                                  Package.Id = Product.PackageId,
                                  Location.Id = Product.LocationId";
                if (DBUtl.ExecSQL(update, product.Id,product.Weight,product.Width, product.Height,
                    product.Depth,product.Type,product.CategoryDescription,
                    product.Isle,product.Shelf,product.ReorderQty)==1)

                {
                    TempData["Message"] = "Product Updated";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
                return RedirectToAction("ListProduct");
            }            
        }
        [Authorize]
        public IActionResult DeleteProduct(int id)
        {
            string select = @"SELECT * FROM Product 
                              WHERE Id={0}";
            DataTable ds = DBUtl.GetTable(select, id);
            if (ds.Rows.Count != 1)
            {
                TempData["Message"] = "Product record no longer exists.";
                TempData["MsgType"] = "warning";
            }
            else
            {
                string delete = "DELETE FROM Product WHERE Id={0}";
                int res = DBUtl.ExecSQL(delete, id);
                if (res == 1)
                {
                    TempData["Message"] = "Product Deleted";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
            }
            return RedirectToAction("ListProduct");
        }

        private SelectList GetListCat()
        {
            // Get a list of all genres from the database
            var catSql = DBUtl.GetList("SELECT Id, CategoryDescription FROM Category");
            SelectList lstCat = new SelectList(catSql, "Id", "CategoryDescription");
            return lstCat;
        }

        private SelectList GetListType()
        {
            
            var typeSql = DBUtl.GetList("SELECT Id, Type FROM Package");
            SelectList lstType = new SelectList(typeSql, "Id", "Type");
            return lstType;
        }
    }
}