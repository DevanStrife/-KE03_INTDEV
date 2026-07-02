using MatrixInc.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace MatrixInc.DataAccess.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly MatrixDbContext _context;

    public OrderRepository(MatrixDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .AsSplitQuery()
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        // Explicitly load addresses for customers that have them
        foreach (var order in orders)
        {
            if (order.Customer?.AddressId != null)
            {
                await _context.Entry(order.Customer)
                    .Reference(c => c.Address)
                    .LoadAsync();
            }
        }

        return orders;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .AsSplitQuery()
            .FirstOrDefaultAsync(o => o.Id == id);

        // Explicitly load address if customer has one
        if (order?.Customer?.AddressId != null)
        {
            await _context.Entry(order.Customer)
                .Reference(c => c.Address)
                .LoadAsync();
        }

        return order;
    }

    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId)
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.CustomerId == customerId)
            .AsSplitQuery()
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        // Explicitly load addresses for customers that have them
        foreach (var order in orders)
        {
            if (order.Customer?.AddressId != null)
            {
                await _context.Entry(order.Customer)
                    .Reference(c => c.Address)
                    .LoadAsync();
            }
        }

        return orders;
    }

    public async Task<int> CreateOrderAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
        return order.Id;
    }

    public async Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }
}
