using E_Commerce.API.DataModels;
using E_Commerce.API.Interfaces;

using E_Commerce.API.Data;
using E_Commerce.API.DataModels;
using E_Commerce.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.API.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ECommerceDbContext _context;
        private readonly ILogger<CartRepository> _logger;

        public CartRepository(ECommerceDbContext context, ILogger<CartRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            _logger.LogInformation("Getting cart for user ID: {UserId}", userId);
            return await _context.Carts
                .Include(c => c.CartItems)
                 .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart> AddItemToCartAsync(string userId, Guid productId, int quantity)
        {
            _logger.LogInformation("Adding item to cart for user ID: {UserId}, product ID: {ProductId}", userId, productId);

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                _logger.LogWarning("Product with ID: {ProductId} not found", productId);
                throw new Exception($"Product with ID: {productId} not found.");
            }

            var cart = await GetCartByUserIdAsync(userId) ?? new Cart { UserId = userId, CartId = Guid.NewGuid() };

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
            {
                cartItem = new CartItem { CartItemId = Guid.NewGuid(), ProductId = productId, Quantity = quantity, CartId = cart.CartId };
                cart.CartItems.Add(cartItem);
                _context.CartItems.Add(cartItem);
            }
            else
            {
                _logger.LogInformation("Updating existing cart item for product ID: {ProductId}", productId);
                cartItem.Quantity += quantity;
                _context.CartItems.Update(cartItem);
            }

            if (_context.Entry(cart).State == EntityState.Detached)
            {
                _context.Carts.Add(cart);
            }

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Item added to cart successfully for user ID: {UserId}, product ID: {ProductId}", userId, productId);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error occurred while adding item to cart for user ID: {UserId}, product ID: {ProductId}", userId, productId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding item to cart for user ID: {UserId}, product ID: {ProductId}", userId, productId);
                throw;
            }

            return cart;
        }


        public async Task<Cart> RemoveItemFromCartAsync(string userId, Guid productId)
        {
            _logger.LogInformation("Removing item from cart for user ID: {UserId}, product ID: {ProductId}", userId, productId);
            var cart = await GetCartByUserIdAsync(userId);
            if (cart == null) return null;

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem != null)
            {
                cart.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            return cart;
        }

        public async Task<Cart> ClearCartAsync(string userId)
        {
            _logger.LogInformation("Clearing cart for user ID: {UserId}", userId);
            var cart = await GetCartByUserIdAsync(userId);
            if (cart == null) return null;

            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<bool> UpdateItemQuantityAsync(string userId, Guid productId, int quantity)
        {
            _logger.LogInformation("Updating item quantity in cart for user ID: {UserId}, product ID: {ProductId}", userId, productId);
            var cart = await GetCartByUserIdAsync(userId);
            if (cart == null) return false;

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null) return false;

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetCartItemCountAsync(string userId)
        {
            _logger.LogInformation("Getting item count in cart for user ID: {UserId}", userId);
            var cart = await GetCartByUserIdAsync(userId);
            if (cart == null) return 0;

            return cart.CartItems.Sum(ci => ci.Quantity);
        }

        public async Task<decimal> GetCartTotalPriceAsync(string userId)
        {
            _logger.LogInformation("Getting total price of items in cart for user ID: {UserId}", userId);
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product) // Include Product
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null) return 0;

            return cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price); // Now Product is included
        }

        public async Task<bool> SaveCartForLaterAsync(string userId)
        {
            _logger.LogInformation("Saving cart for later for user ID: {UserId}", userId);
            var cart = await GetCartByUserIdAsync(userId);
            if (cart == null) return false;

            cart.IsSavedForLater = true; // Assuming you add this property to Cart model
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Cart> RestoreSavedCartAsync(string userId)
        {
            _logger.LogInformation("Restoring saved cart for user ID: {UserId}", userId);
            var cart = await GetCartByUserIdAsync(userId);
            if (cart == null || !cart.IsSavedForLater) return null;

            cart.IsSavedForLater = false; // Assuming you add this property to Cart model
            await _context.SaveChangesAsync();
            return cart;
        }
    }
}

