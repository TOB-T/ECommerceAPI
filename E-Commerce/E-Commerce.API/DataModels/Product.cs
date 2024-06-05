using E_Commerce.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.API.DataModels
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        // Additional fields like ImageURL, Category, etc., can be added here
        [Required]
        [StringLength(255)]
        public string ImageUrl { get; set; }

        [Required]
        public ProductCategory Category { get; set; }
    }
}
