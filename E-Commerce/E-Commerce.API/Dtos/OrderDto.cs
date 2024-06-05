namespace E_Commerce.API.Dtos
{
    public class OrderDto
    {
        public string UserId { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; }
    }
}
