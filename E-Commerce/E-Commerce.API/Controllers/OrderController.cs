using AutoMapper;
using E_Commerce.API.DataModels;
using E_Commerce.API.Dtos;
using E_Commerce.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderInterface _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderInterface orderRepository, IMapper mapper, ILogger<OrderController> logger)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
       // [Authorize] // Optional, remove if you don't have authentication yet
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto orderDto)
        {
            _logger.LogInformation("Received request to create a new order for user ID: {UserId}", orderDto.UserId);

            // Check if user exists
            if (!await _orderRepository.UserExistsAsync(orderDto.UserId))
            {
                _logger.LogWarning("Invalid UserId: {UserId}. User does not exist.", orderDto.UserId);
                return BadRequest("Invalid UserId. User does not exist.");
            }

            // Step 1: Map the incoming OrderDto to an Order entity using AutoMapper
            var order = _mapper.Map<Order>(orderDto);
            order.OrderId = Guid.NewGuid(); // Generate new GUID for OrderId

            try
            {
                // Step 2: Use the repository to create the order in the database
                var createdOrder = await _orderRepository.CreateOrderAsync(order);

                // Step 3: Map the created order entity back to an OrderDto
                var createdOrderDto = _mapper.Map<OrderDto>(createdOrder);

                // Step 4: Return a 201 Created response with the created order DTO
                _logger.LogInformation("Order created successfully for user ID: {UserId}", orderDto.UserId);
                return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.OrderId }, createdOrderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new order for user ID: {UserId}", orderDto.UserId);
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            _logger.LogInformation("Received request to get order with ID: {OrderId}", id);

            try
            {
                var order = await _orderRepository.GetOrderByIdAsync(id);
                if (order == null)
                {
                    _logger.LogWarning("Order with ID: {OrderId} not found", id);
                    return NotFound();
                }
                var orderDto = _mapper.Map<OrderDto>(order);
                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the order with ID: {OrderId}", id);
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            _logger.LogInformation("Received request to get all orders");

            try
            {
                var orders = await _orderRepository.GetAllOrdersAsync();
                var orderDtos = _mapper.Map<List<OrderDto>>(orders);
                return Ok(orderDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all orders");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] OrderDto orderDto)
        {
            _logger.LogInformation("Received request to update order with ID: {OrderId}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid order model state");
                return BadRequest(ModelState);
            }

            try
            {
                var order = await _orderRepository.GetOrderByIdAsync(id);
                if (order == null)
                {
                    _logger.LogWarning("Order with ID: {OrderId} not found", id);
                    return NotFound();
                }

                // Update the order entity with the values from orderDto
                order.Status = orderDto.Status;
                order.OrderDate = orderDto.OrderDate;
                order.OrderDetail = _mapper.Map<List<OrderDetail>>(orderDto.OrderDetails);

                await _orderRepository.UpdateOrderAsync(order);

                var updatedOrderDto = _mapper.Map<OrderDto>(order);
                return Ok(updatedOrderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the order with ID: {OrderId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            _logger.LogInformation("Received request to delete order with ID: {OrderId}", id);

            try
            {
                var order = await _orderRepository.GetOrderByIdAsync(id);
                if (order == null)
                {
                    _logger.LogWarning("Order with ID: {OrderId} not found", id);
                    return NotFound();
                }

                var success = await _orderRepository.DeleteOrderAsync(id);
                if (!success)
                {
                    _logger.LogWarning("Failed to delete order with ID: {OrderId}", id);
                    return BadRequest();
                }

                _logger.LogInformation("Order with ID: {OrderId} deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the order with ID: {OrderId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
