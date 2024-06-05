using E_Commerce.API.DataModels;

namespace E_Commerce.API.Interfaces
{
    public interface IOrderInterface
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<Order> GetOrderByIdAsync(Guid orderId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task<Order> UpdateOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(Guid orderId);
        Task<bool> UserExistsAsync(string userId); // Add UserExistsAsync method
    }
}
