using E_Commerce.API.DataModels;

namespace E_Commerce.API.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart> GetCartByUserIdAsync(string userId);
        Task<Cart> AddItemToCartAsync(string userId, Guid productId, int quantity);
        Task<Cart> RemoveItemFromCartAsync(string userId, Guid productId);
        Task<Cart> ClearCartAsync(string userId);
        Task<bool> UpdateItemQuantityAsync(string userId, Guid productId, int quantity);
        Task<int> GetCartItemCountAsync(string userId);
        Task<decimal> GetCartTotalPriceAsync(string userId);
        Task<bool> SaveCartForLaterAsync(string userId);
        Task<Cart> RestoreSavedCartAsync(string userId);
    }
}
