using System;
using System.ComponentModel.DataAnnotations;
using E_Commerce.API.DataModels;

namespace E_Commerce.API.ServiceModels
{
    public class CartItem
    {
        public Guid CartItemId { get; set; }
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Cart Cart { get; set; }
        public Product Product { get; set; } // Add this navigation property

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
