using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyProject.DataModel;

namespace MyProject.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18, 2)")]
        [Range(0.01, 100000000)]
        public decimal Price { get; set; }

        public User User { get; set; }

        public int UserId { get; set; }
    }
}
