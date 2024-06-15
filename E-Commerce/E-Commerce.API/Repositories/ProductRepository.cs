using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using E_Commerce.API.Data;
using E_Commerce.API.DataModels;
using E_Commerce.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ECommerceDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(ECommerceDbContext context, IWebHostEnvironment environment, ILogger<ProductRepository> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            _logger.LogInformation("Creating a new product: {ProductName}", product.Name);

            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product created successfully: {ProductName}", product.Name);
                return product;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new product: {ProductName}", product.Name);
                throw;
            }
        }

        public async Task<Product> GetProductByIdAsync(Guid productId)
        {
            _logger.LogInformation("Getting product with ID: {ProductId}", productId);

            try
            {
                var product = await _context.Products.FindAsync(productId);

                if (product == null)
                {
                    _logger.LogWarning("Product with ID: {ProductId} not found", productId);
                }

                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the product with ID: {ProductId}", productId);
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            _logger.LogInformation("Getting all products");

            try
            {
                return await _context.Products.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all products");
                throw;
            }
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            _logger.LogInformation("Updating product with ID: {ProductId}", product.ProductId);

            try
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product with ID: {ProductId} updated successfully", product.ProductId);
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the product with ID: {ProductId}", product.ProductId);
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(Guid productId)
        {
            _logger.LogInformation("Deleting product with ID: {ProductId}", productId);

            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID: {ProductId} not found", productId);
                    return false;
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product with ID: {ProductId} deleted successfully", productId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the product with ID: {ProductId}", productId);
                throw;
            }
        }

        public async Task<string> UploadProductImageAsync(IFormFile file, Guid productId)
        {
            _logger.LogInformation("Uploading image for product with ID: {ProductId}", productId);

            try
            {
                var webRootPath = _environment.WebRootPath;
                if (string.IsNullOrEmpty(webRootPath))
                {
                    throw new ArgumentNullException(nameof(webRootPath), "Web root path is not set.");
                }

                var uploadsFolder = Path.Combine(webRootPath, "Images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                _logger.LogInformation("Image uploaded successfully to {FilePath}", filePath);

                var product = await GetProductByIdAsync(productId);
                if (product != null)
                {
                    product.ImageUrl = $"/Images/{uniqueFileName}";
                    await UpdateProductAsync(product);
                    _logger.LogInformation("Product image URL updated successfully for product with ID: {ProductId}", productId);
                }
                else
                {
                    _logger.LogWarning("Product with ID: {ProductId} not found when attempting to update image URL", productId);
                }

                return $"/Images/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading the image for product with ID: {ProductId}", productId);
                throw;
            }
        }


    }
}

