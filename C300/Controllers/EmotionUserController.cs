using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using C300.Models;
using Microsoft.AspNetCore.Mvc;


namespace C300.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmotionUserController : ControllerBase
    {
        // GET api/emotion
        [HttpGet]

        public IEnumerable<Product> Get()
        {
            List<Product> dbList = DBUtl.GetList<Product>("SELECT Product.Id, Description, Weight, Width, Height, Depth, Type, CategoryDescription, Isle, Shelf, ReorderQty " +
                "FROM Product, Package, Category, Location"+ 
                    "WHERE Product.PackageId = Package.Id"+
                    "AND Product.CategoryId = Category.Id"+
                    "AND Product.LocationId = Location.Id");
            return dbList;
        }

        
        [HttpGet("{name}")]
        public IActionResult Get(string name)
        {
            string productSql = @"SELECT Product.Id, Description, Weight, Width, Height, Depth, Type, CategoryDescription, Isle, Shelf, ReorderQty
                    FROM Product, Package, Category, Location 
                    WHERE Product.PackageId = Package.Id
                    AND Product.CategoryId = Category.Id
                    AND Product.LocationId = Location.Id
                    AND Description = '{0}'";
            List<Product> dbList = DBUtl.GetList<Product>(productSql, name);
            if (dbList.Count > 0)
                return Ok(dbList);
            else
                return NotFound();
        }

        // POST api/emotion
        [HttpPost]
        public IActionResult Post([FromBody] Product newProduct)
        {
            if (newProduct == null)
            {
                return BadRequest();
            }

            string sqlInsert = @"INSERT INTO Product(Description, Weight, Width, Height, Depth, PackageId, CategoryId, LocationId, ReorderQty) 
                 VALUES('{0}', {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})";
            if (DBUtl.ExecSQL(sqlInsert, newProduct.Description, newProduct.Weight, newProduct.Width,
                                           newProduct.Height, newProduct.Depth, newProduct.PackageId,
                                           newProduct.CategoryId, newProduct.LocationId, newProduct.ReorderQty) == 1)
                return Ok();
            else
                return BadRequest(new { Message = DBUtl.DB_Message });
        }

       
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] Product product)
        {
            if (product == null || id == null)
            {
                return BadRequest();
            }

            string sql = @"UPDATE Product 
                              SET Product.Weight={1}, Product.Width={2}, Product.Height={3}, Product.Depth={4}, Package.Type={5},
                                  Category.CategoryDescription = '{6}', Location.Isle = '{7}', Location.Shelf = '{8}', ReorderQty = {9}              
                              WHERE Product.Id={0} 
                              AND Category.Id = Product.CategoryId, 
                                  Package.Id = Product.PackageId,
                                  Location.Id = Product.LocationId";
            string update = String.Format(sql, product.Id, product.Weight, product.Width, product.Height,
                    product.Depth, product.Type, product.CategoryDescription,
                    product.Isle, product.Shelf, product.ReorderQty);
            if (DBUtl.ExecSQL(update) == 1)
                return Ok();
            else
                return BadRequest(new { Message = DBUtl.DB_Message });
        }

        
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {

            string sql = @"DELETE Product 
                         WHERE Id={0}";
            string delete = String.Format(sql, id);
            if (DBUtl.ExecSQL(delete) == 1)
                return Ok();
            else
                return BadRequest(new { Message = DBUtl.DB_Message });

        }
    }
}

