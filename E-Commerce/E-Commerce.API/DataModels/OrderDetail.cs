using System.ComponentModel.DataAnnotations;

namespace E_Commerce.API.DataModels
{
    public class OrderDetail
    {
        [Key]
        public Guid OrderDetailId { get; set; }

        [Required]
        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }

        [Required]
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
