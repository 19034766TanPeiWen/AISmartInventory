using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using C300.Models;

namespace C300.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DemoController : ControllerBase
    {

        private IWebHostEnvironment _env;
        public DemoController(IWebHostEnvironment environment)
        {
            _env = environment;
        }

        private string DoPhotoUpload(IFormFile photo)
        {
            string fext = Path.GetExtension(photo.FileName);
            string uname = Guid.NewGuid().ToString();
            string fname = uname + fext;
            string fullpath = Path.Combine(_env.WebRootPath, "candidates/" + fname);
            FileStream fs = new FileStream(fullpath, FileMode.Create);
            photo.CopyTo(fs);
            fs.Close();
            return fname;
        }

        // GET api/demo
        [HttpGet]

        public IEnumerable<UserDetails> Get()
        {
            List<UserDetails> dbList = DBUtl.GetList<UserDetails>("SELECT * FROM Employee");
            return dbList;
        }

        // GET api/demo/batman
        [HttpGet("{name}")]
        public IActionResult Get(string name)
        {

            List<UserDetails> dbList = DBUtl.GetList<UserDetails>($"SELECT * FROM Employee WHERE EmployeeNo='{name}'");
            if (dbList.Count > 0)
                return Ok(dbList[0]);
            else
                return NotFound();
        }

        [HttpPost("upload", Name = "upload")]
        public IActionResult UploadFile([FromForm] string EmployeeNo,
            [FromForm] string LastName,
            [FromForm] string GivenName,
            [FromForm] string OtherNames,
            [FromForm] DateTime dob,
            [FromForm] int CompanyId)
        {
            if (EmployeeNo == null || LastName == null || GivenName == null || OtherNames == null || dob == null || CompanyId.ToString() == null) 
            {
                return BadRequest("Parameters must not be null");
            }

            string sqlInsert = @"INSERT INTO Employee(EmployeeNo, LastName, GivenName, OtherNames, dob, CompanyId) VALUES
                        ('{0}','{1}','{2}','{3}','{4:yyyy-MM-dd}',{5} );";
            if (DBUtl.ExecSQL(sqlInsert, EmployeeNo, LastName, GivenName, OtherNames, dob, CompanyId) == 1)
                return Ok();
            else
                return BadRequest(new { Message = DBUtl.DB_Message });
        }

        // DELETE api/demo/wonderwoman
        [HttpDelete("{id}")]
        public IActionResult Delete(string no)
        {
            if (no == null)
            {
                return BadRequest();
            }

            string sql = @"DELETE Employee 
                         WHERE EmployeeNo='{0}'";
            string delete = String.Format(sql, no);
            if (DBUtl.ExecSQL(delete) == 1)
                return Ok();
            else
                return BadRequest(new { Message = DBUtl.DB_Message });

        }
    }
}

