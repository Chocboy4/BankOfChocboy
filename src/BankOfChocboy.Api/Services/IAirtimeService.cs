
using BankOfChocboy.Application.DTOs;

namespace  BankOfChocboy.Services
{
    public interface IAirtimeService
    {
        Task<AirtimePurchaseResponseDto> PurchaseAirtimeAsync(string accountNumber, AirtimePurchaseRequestDto request);
    }
}
