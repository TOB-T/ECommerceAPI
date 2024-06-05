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

        public OrderController(IOrderInterface orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        [HttpPost]
       // [Authorize] // Optional, remove if you don't have authentication yet
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto orderDto)
        {
            // Check if user exists
            if (!await _orderRepository.UserExistsAsync(orderDto.UserId))
            {
                return BadRequest("Invalid UserId. User does not exist.");
            }

            // Step 1: Map the incoming OrderDto to an Order entity using AutoMapper
            var order = _mapper.Map<Order>(orderDto);
            order.OrderId = Guid.NewGuid(); // Generate new GUID for OrderId

            // Step 2: Use the repository to create the order in the database
            var createdOrder = await _orderRepository.CreateOrderAsync(order);

            // Step 3: Map the created order entity back to an OrderDto
            var createdOrderDto = _mapper.Map<OrderDto>(createdOrder);

            // Step 4: Return a 201 Created response with the created order DTO
            return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.OrderId }, createdOrderDto);
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            var orderDto = _mapper.Map<OrderDto>(order);
            return Ok(orderDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            var orderDtos = _mapper.Map<List<OrderDto>>(orders);
            return Ok(orderDtos);
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] OrderDto orderDto)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
            {
                return BadRequest();
            }
            _mapper.Map(orderDto, order);
            await _orderRepository.UpdateOrderAsync(order);
            var updatedOrderDto = _mapper.Map<OrderDto>(order);
            return Ok(updatedOrderDto);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            var success = await _orderRepository.DeleteOrderAsync(id);
            if (!success)
            {
                return BadRequest();
            }
            return NoContent();
        }
    }
}
