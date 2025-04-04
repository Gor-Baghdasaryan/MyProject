using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyProject.Models;

namespace MyProject.DataModel
{
    public class ProductModel
    {
        [Required]
        [MinLength(1, ErrorMessage = "Name Cannot be Empty.")]
        public string Name { get; set; } = String.Empty;

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price Must Be Greater Than 0.")]
        public decimal Price { get; set; }
    }
}
