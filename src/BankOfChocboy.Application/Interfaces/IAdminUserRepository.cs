using BankOfChocboy.Domain.Models;

namespace BankOfChocboy.Application.Interfaces
{
    public interface IAdminUserRepository
    {
        Task<Admin?> GetByUsernameAsync(string username);
        Task<bool> AnyAsync();
        Task CreateAsync(Admin admin);
    }
}
