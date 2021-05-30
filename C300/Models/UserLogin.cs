using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace C300.Models
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Please enter Employee ID")]
        public string EmployeeNo { get; set; }

        public int CompanyId { get; set; }

        public bool RememberMe { get; set; }
    }
}
