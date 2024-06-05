using AutoMapper;
using E_Commerce.API.DataModels;
using E_Commerce.API.Dtos;
using E_Commerce.API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductController(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto productDto)
        {

            var product = _mapper.Map<Product>(productDto);
            product.ProductId = Guid.NewGuid(); // Generate new GUID for ProductId
            var createdProduct = await _productRepository.CreateProductAsync(product);
            var createdProductDto = _mapper.Map<ProductDto>(createdProduct);
            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.ProductId }, createdProductDto);
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productRepository.GetAllProductsAsync();
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(productDtos);
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductDto productDto)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }
            _mapper.Map(productDto, existingProduct);
            var updatedProduct = await _productRepository.UpdateProductAsync(existingProduct);
            var updatedProductDto = _mapper.Map<ProductDto>(updatedProduct);
            return Ok(updatedProductDto);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var deleted = await _productRepository.DeleteProductAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadProductImage([FromForm] ProductImageUploadDto model)
        {
            var product = await _productRepository.GetProductByIdAsync(model.ProductId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var imageUrl = await _productRepository.UploadProductImageAsync(model.Image, model.ProductId);
            return Ok(new { ImageUrl = imageUrl });
        }
    }
}

