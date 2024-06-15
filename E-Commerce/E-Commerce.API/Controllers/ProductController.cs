using AutoMapper;
using E_Commerce.API.DataModels;
using E_Commerce.API.Dtos;
using E_Commerce.API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductRepository productRepository, IMapper mapper, ILogger<ProductController> logger)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto productDto)
        {
            _logger.LogInformation("Received request to create a new product");

            var product = _mapper.Map<Product>(productDto);
            product.ProductId = Guid.NewGuid(); // Generate new GUID for ProductId

            try
            {
                var createdProduct = await _productRepository.CreateProductAsync(product);
                var createdProductDto = _mapper.Map<ProductDto>(createdProduct);
                _logger.LogInformation("Product created successfully with ID: {ProductId}", createdProduct.ProductId);
                return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.ProductId }, createdProductDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new product");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            _logger.LogInformation("Received request to get product with ID: {ProductId}", id);

            try
            {
                var product = await _productRepository.GetProductByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID: {ProductId} not found", id);
                    return NotFound();
                }
                var productDto = _mapper.Map<ProductDto>(product);
                return Ok(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the product with ID: {ProductId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            _logger.LogInformation("Received request to get all products");

            try
            {
                var products = await _productRepository.GetAllProductsAsync();
                var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all products");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductDto productDto)
        {
            _logger.LogInformation("Received request to update product with ID: {ProductId}", id);

            try
            {
                var existingProduct = await _productRepository.GetProductByIdAsync(id);
                if (existingProduct == null)
                {
                    _logger.LogWarning("Product with ID: {ProductId} not found", id);
                    return NotFound();
                }
                _mapper.Map(productDto, existingProduct);
                var updatedProduct = await _productRepository.UpdateProductAsync(existingProduct);
                var updatedProductDto = _mapper.Map<ProductDto>(updatedProduct);
                _logger.LogInformation("Product with ID: {ProductId} updated successfully", id);
                return Ok(updatedProductDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the product with ID: {ProductId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            _logger.LogInformation("Received request to delete product with ID: {ProductId}", id);

            try
            {
                var deleted = await _productRepository.DeleteProductAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning("Product with ID: {ProductId} not found", id);
                    return NotFound();
                }
                _logger.LogInformation("Product with ID: {ProductId} deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the product with ID: {ProductId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadProductImage([FromForm] ProductImageUploadDto model)
        {
            _logger.LogInformation("Received request to upload image for product with ID: {ProductId}", model.ProductId);

            try
            {
                var product = await _productRepository.GetProductByIdAsync(model.ProductId);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID: {ProductId} not found", model.ProductId);
                    return NotFound("Product not found.");
                }

                var imageUrl = await _productRepository.UploadProductImageAsync(model.Image, model.ProductId);
                _logger.LogInformation("Image uploaded successfully for product with ID: {ProductId}", model.ProductId);
                return Ok(new { ImageUrl = imageUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading the image for product with ID: {ProductId}", model.ProductId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}


