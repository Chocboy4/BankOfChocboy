using BankOfChocboy.Application.DTOs;

namespace BankOfChocboy.Application.Interfaces
{
    public interface IAirtimeVendRepository
    {
        Task<AirtimeVendResult> RecordAsync(AirtimeVendRequestDto request);
    }
}
