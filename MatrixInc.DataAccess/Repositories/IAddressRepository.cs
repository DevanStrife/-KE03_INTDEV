using MatrixInc.DataAccess.Models;

namespace MatrixInc.DataAccess.Repositories;

public interface IAddressRepository
{
    Task<Address?> GetByIdAsync(int id);
    Task<int> AddAsync(Address address);
    Task UpdateAsync(Address address);
}
