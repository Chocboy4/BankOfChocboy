using BankOfChocboy.Application.DTOs;

namespace BankOfChocboy.Application.Interfaces
{
    public interface IAirtimeRepository
    {
        Task<AirtimePurchaseResponseDto> PurchaseAsync(string accountNumber,
            string phoneNumber,
            string network,
            decimal amount);
    }
}
