using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace C300.Models
{
    public class Product
    {   public int Id { get; set; }

        [Required(ErrorMessage ="Enter a description")]
        [StringLength(50, ErrorMessage = "Maximum is 50 Characters")]
        public char Description { get; set; }

        public float Weight { get; set; }

        public float Width{ get; set; }

        public float Height { get; set; }

        public float Depth { get; set; }

        public int PackageId { get; set; }

        public char Type { get; set; }

        public int CategoryId{ get; set; }

        [Required(ErrorMessage = "Enter a description")]
        [StringLength(50, ErrorMessage = "Maximum is 50 Characters")]
        public char CategoryDescription { get; set; }

        public int LocationId { get; set; }

        public char Isle { get; set; }

        public char Shelf { get; set; }

        public int ReorderQty { get; set; }
    
        
    }
}
