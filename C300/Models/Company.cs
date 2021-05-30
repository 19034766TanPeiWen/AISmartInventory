using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace C300.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter Trading As")]
        [StringLength(50, ErrorMessage = "Maximum is 50 Characters")]
        public string TradingAs { get; set; }

        [Required(ErrorMessage = "Enter EUN")]
        [StringLength(50, ErrorMessage = "Maximum is 50 Characters")]
        public string UEN { get; set; }

        public DateTime IncorporationDate { get; set; }

        [Required(ErrorMessage = "Enter a Registered Office")]
        public string RegisteredOffice { get; set; }

    }
}
