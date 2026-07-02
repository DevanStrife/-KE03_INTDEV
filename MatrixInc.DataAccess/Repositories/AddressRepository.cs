using MatrixInc.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace MatrixInc.DataAccess.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly MatrixDbContext _context;

    public AddressRepository(MatrixDbContext context)
    {
        _context = context;
    }

    public async Task<Address?> GetByIdAsync(int id)
    {
        return await _context.Addresses.FindAsync(id);
    }

    public async Task<int> AddAsync(Address address)
    {
        await _context.Addresses.AddAsync(address);
        await _context.SaveChangesAsync();
        return address.Id;
    }

    public async Task UpdateAsync(Address address)
    {
        _context.Addresses.Update(address);
        await _context.SaveChangesAsync();
    }
}
