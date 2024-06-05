using E_Commerce.API.DataModels;

namespace E_Commerce.API.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> CreateProductAsync(Product product);
        Task<Product> GetProductByIdAsync(Guid productId);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(Guid productId);
        Task<string> UploadProductImageAsync(IFormFile file, Guid productId);

    }
}
