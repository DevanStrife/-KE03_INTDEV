using MatrixInc.Api.DTOs;
using MatrixInc.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MatrixInc.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderRepository orderRepository, ILogger<OrdersController> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    /// <summary>
    /// Haal alle bestellingen op
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
    {
        try
        {
            var orders = await _orderRepository.GetAllAsync();
            var orderDtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer?.Name ?? "Onbekend",
                CustomerEmail = o.Customer?.Email ?? "",
                CustomerPhone = o.Customer?.PhoneNumber ?? "",
                CustomerAddress = o.Customer?.Address?.FullAddress ?? "Geen adres",
                Street = o.Customer?.Address?.Street ?? "",
                HouseNumber = o.Customer?.Address?.HouseNumber ?? "",
                City = o.Customer?.Address?.City ?? "",
                PostalCode = o.Customer?.Address?.PostalCode ?? "",
                Province = o.Customer?.Address?.Province ?? "",
                OrderDate = o.OrderDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                Notes = o.Notes,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? "Onbekend",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            });

            return Ok(orderDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all orders");
            return StatusCode(500, $"Er is een fout opgetreden bij het ophalen van bestellingen: {ex.Message}");
        }
    }

    /// <summary>
    /// Haal alleen bestellingen op die klaar zijn voor levering of onderweg zijn (voor courier app)
    /// </summary>
    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetPendingOrders()
    {
        try
        {
            var orders = await _orderRepository.GetAllAsync();
            var pendingOrders = orders
                .Where(o => o.Status == "Klaar voor Levering" || o.Status == "Onderweg")
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    CustomerId = o.CustomerId,
                    CustomerName = o.Customer?.Name ?? "Onbekend",
                    CustomerEmail = o.Customer?.Email ?? "",
                    CustomerPhone = o.Customer?.PhoneNumber ?? "",
                    CustomerAddress = o.Customer?.Address?.FullAddress ?? "Geen adres",
                    Street = o.Customer?.Address?.Street ?? "",
                    HouseNumber = o.Customer?.Address?.HouseNumber ?? "",
                    City = o.Customer?.Address?.City ?? "",
                    PostalCode = o.Customer?.Address?.PostalCode ?? "",
                    Province = o.Customer?.Address?.Province ?? "",
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    Notes = o.Notes,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product?.Name ?? "Onbekend",
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice
                    }).ToList()
                });

            return Ok(pendingOrders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending orders");
            return StatusCode(500, "Er is een fout opgetreden bij het ophalen van bestellingen");
        }
    }

    /// <summary>
    /// Haal een specifieke bestelling op
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null)
            {
                return NotFound($"Bestelling met ID {id} niet gevonden");
            }

            var orderDto = new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer?.Name ?? "Onbekend",
                CustomerEmail = order.Customer?.Email ?? "",
                CustomerPhone = order.Customer?.PhoneNumber ?? "",
                CustomerAddress = order.Customer?.Address?.FullAddress ?? order.Customer?.AddressOld ?? "Geen adres",
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                Notes = order.Notes,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? "Onbekend",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };

            return Ok(orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order {OrderId}", id);
            return StatusCode(500, "Er is een fout opgetreden bij het ophalen van de bestelling");
        }
    }

    /// <summary>
    /// Update de status van een bestelling
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<ActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto dto)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null)
            {
                return NotFound($"Bestelling met ID {id} niet gevonden");
            }

            // Valideer status
            var validStatuses = new[] { "In behandeling", "Verzonden", "Afgeleverd", "Geannuleerd", "Klaar voor Levering", "Onderweg" };
            if (!validStatuses.Contains(dto.Status))
            {
                return BadRequest($"Ongeldige status. Geldige waarden: {string.Join(", ", validStatuses)}");
            }

            order.Status = dto.Status;
            await _orderRepository.UpdateAsync(order);

            _logger.LogInformation("Order {OrderId} status updated to {Status}", id, dto.Status);

            return Ok(new { message = $"Status bijgewerkt naar: {dto.Status}" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order {OrderId} status", id);
            return StatusCode(500, "Er is een fout opgetreden bij het bijwerken van de status");
        }
    }
}
