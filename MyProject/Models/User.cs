using System.ComponentModel.DataAnnotations;

namespace MyProject.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
