using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace C300.Models
{
    public class UserDetails
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter Employee No")]
        public string EmployeeNo { get; set; }

        [Required(ErrorMessage = "Please enter Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter Given Name")]
        public string GivenName { get; set; }

        public string OtherNames { get; set; }

        [Required(ErrorMessage = "Please enter Dob")]
        [DataType(DataType.Date)]
        public DateTime Dob { get; set; }

        public int CompanyId { get; set; }

        public string TradingAs { get; set; }

    }
}
