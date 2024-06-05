using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.API.DataModels
{
    public class Order
    {

        [Key]
        public Guid OrderId { get; set; }

        [Required]
        public string UserId { get; set; }

        public string Status { get; set; } = "Pending";

        [Required]
        public DateTime OrderDate { get; set; }

        public virtual IdentityUser User { get; set; }

        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
