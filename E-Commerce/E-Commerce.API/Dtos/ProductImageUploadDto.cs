namespace E_Commerce.API.Dtos
{
    public class ProductImageUploadDto
    {
        public Guid ProductId { get; set; }
        public IFormFile Image { get; set; }
    }
}
