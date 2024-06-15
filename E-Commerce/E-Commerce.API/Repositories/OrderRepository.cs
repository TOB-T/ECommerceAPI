using E_Commerce.API.Data;
using E_Commerce.API.DataModels;
using E_Commerce.API.Interfaces;
using E_Commerce.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System.Security.Claims;
using System;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.API.Repositories
{
    public class OrderRepository : IOrderInterface
    {
        private readonly ECommerceDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(ECommerceDbContext context, UserManager<IdentityUser> userManager, ILogger<OrderRepository> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {

            _logger.LogInformation("Creating a new order for user ID: {UserId}", order.UserId);

            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Order created successfully for user ID: {UserId}", order.UserId);
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new order for user ID: {UserId}", order.UserId);
                throw;
            }


        }

        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            _logger.LogInformation("Getting order with ID: {OrderId}", orderId);

            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderDetail)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                {
                    _logger.LogWarning("Order with ID: {OrderId} not found", orderId);
                }

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the order with ID: {OrderId}", orderId);
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            _logger.LogInformation("Getting all orders");

            try
            {
                return await _context.Orders
                    .Include(o => o.OrderDetail)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all orders");
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            _logger.LogInformation("Getting orders for user ID: {UserId}", userId);

            try
            {
                return await _context.Orders
                    .Include(o => o.OrderDetail)
                    .Where(o => o.UserId == userId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting orders for user ID: {UserId}", userId);
                throw;
            }
        }

        public async Task<Order> UpdateOrderAsync(Order order)
        {
            _logger.LogInformation("Updating order with ID: {OrderId}", order.OrderId);

            try
            {
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Order with ID: {OrderId} updated successfully", order.OrderId);
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the order with ID: {OrderId}", order.OrderId);
                throw;
            }
        }


        public async Task<bool> DeleteOrderAsync(Guid orderId)
        {
            _logger.LogInformation("Deleting order with ID: {OrderId}", orderId);

            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("Order with ID: {OrderId} not found", orderId);
                    return false;
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Order with ID: {OrderId} deleted successfully", orderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the order with ID: {OrderId}", orderId);
                throw;
            }
        }

        public async Task<bool> UserExistsAsync(string userId)
        {
            _logger.LogInformation("Checking if user ID: {UserId} exists", userId);

            try
            {
                return await _userManager.FindByIdAsync(userId) != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if user ID: {UserId} exists", userId);
                throw;
            }
        }
    }
}
