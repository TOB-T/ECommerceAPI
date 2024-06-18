namespace E_Commerce.API.Dtos
{
    public class CartDto
    {
        public Guid CartId { get; set; }
        public string UserId { get; set; }
        public ICollection<CartItemDto> CartItems { get; set; }
    }
}
