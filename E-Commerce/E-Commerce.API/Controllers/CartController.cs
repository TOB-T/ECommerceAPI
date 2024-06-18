using AutoMapper;
using E_Commerce.API.DataModels;
using E_Commerce.API.Dtos;
using E_Commerce.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartRepository cartRepository, IMapper mapper, ILogger<CartController> logger)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCartByUserId(string userId)
        {
            _logger.LogInformation("Received request to get cart for user ID: {UserId}", userId);

            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                _logger.LogWarning("Cart not found for user ID: {UserId}", userId);
                return NotFound();
            }
            var cartDto = _mapper.Map<CartDto>(cart);
            return Ok(cartDto);
        }

        [HttpPost("{userId}/items")]
        public async Task<IActionResult> AddItemToCart(string userId, [FromBody] CartItemDto cartItemDto)
        {
            //_logger.LogInformation("Received request to add item to cart for user ID: {UserId}", userId);
            //if (cartItemDto.Quantity <= 0)
            //{
            //    return BadRequest("Quantity must be greater than zero.");
            //}

            //var cart = await _cartRepository.AddItemToCartAsync(userId, cartItemDto.ProductId, cartItemDto.Quantity);
            //var cartDto = _mapper.Map<CartDto>(cart);
            //return Ok(cartDto);

            _logger.LogInformation("Received request to add item to cart for user ID: {UserId}", userId);
            if (cartItemDto.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            try
            {
                var cart = await _cartRepository.AddItemToCartAsync(userId, cartItemDto.ProductId, cartItemDto.Quantity);
                var cartDto = _mapper.Map<CartDto>(cart);
                return Ok(cartDto);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error occurred while adding item to cart for user ID: {UserId}, product ID: {ProductId}", userId, cartItemDto.ProductId);
                return StatusCode(500, "A concurrency error occurred. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding item to cart for user ID: {UserId}, product ID: {ProductId}", userId, cartItemDto.ProductId);
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        [HttpDelete("{userId}/items/{productId:Guid}")]
        public async Task<IActionResult> RemoveItemFromCart(string userId, Guid productId)
        {
            _logger.LogInformation("Received request to remove item from cart for user ID: {UserId}, product ID: {ProductId}", userId, productId);

            var cart = await _cartRepository.RemoveItemFromCartAsync(userId, productId);
            if (cart == null)
            {
                _logger.LogWarning("Cart not found for user ID: {UserId}", userId);
                return NotFound();
            }
            var cartDto = _mapper.Map<CartDto>(cart);
            return Ok(cartDto);
        }

        [HttpDelete("{userId}/clear")]
        public async Task<IActionResult> ClearCart(string userId)
        {
            _logger.LogInformation("Received request to clear cart for user ID: {UserId}", userId);

            var cart = await _cartRepository.ClearCartAsync(userId);
            if (cart == null)
            {
                _logger.LogWarning("Cart not found for user ID: {UserId}", userId);
                return NotFound();
            }
            var cartDto = _mapper.Map<CartDto>(cart);
            return Ok(cartDto);
        }

        [HttpGet("{userId}/itemcount")]
        public async Task<IActionResult> GetCartItemCount(string userId)
        {
            _logger.LogInformation("Received request to get item count in cart for user ID: {UserId}", userId);

            var itemCount = await _cartRepository.GetCartItemCountAsync(userId);
            return Ok(new { ItemCount = itemCount });
        }

        [HttpGet("{userId}/totalprice")]
        public async Task<IActionResult> GetCartTotalPrice(string userId)
        {
            _logger.LogInformation("Received request to get total price of items in cart for user ID: {UserId}", userId);

            var totalPrice = await _cartRepository.GetCartTotalPriceAsync(userId);
            return Ok(new { TotalPrice = totalPrice });
        }

        [HttpPost("{userId}/save")]
        public async Task<IActionResult> SaveCartForLater(string userId)
        {
            _logger.LogInformation("Received request to save cart for user ID: {UserId}", userId);

            var success = await _cartRepository.SaveCartForLaterAsync(userId);
            if (!success)
            {
                return NotFound("Cart not found.");
            }

            return Ok();
        }

        [HttpPost("{userId}/restore")]
        public async Task<IActionResult> RestoreSavedCart(string userId)
        {
            _logger.LogInformation("Received request to restore saved cart for user ID: {UserId}", userId);

            var cart = await _cartRepository.RestoreSavedCartAsync(userId);
            if (cart == null)
            {
                return NotFound("Saved cart not found.");
            }

            var cartDto = _mapper.Map<CartDto>(cart);
            return Ok(cartDto);
        }

    }
}

