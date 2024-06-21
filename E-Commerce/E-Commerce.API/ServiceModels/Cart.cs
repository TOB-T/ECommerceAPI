namespace E_Commerce.API.ServiceModels
{
    public class Cart
    {
        public Guid CartId { get; set; }
        public string UserId { get; set; }
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public bool IsSavedForLater { get; set; } // Add this property
    }
}
