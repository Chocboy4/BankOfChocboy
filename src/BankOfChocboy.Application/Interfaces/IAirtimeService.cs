
using BankOfChocboy.Application.DTOs;

namespace BankOfChocboy.Application.Interfaces
{
    public interface IAirtimeService
    {
        Task<AirtimePurchaseResponseDto> PurchaseAirtimeAsync(string accountNumber, AirtimePurchaseRequestDto request);
    }
}
