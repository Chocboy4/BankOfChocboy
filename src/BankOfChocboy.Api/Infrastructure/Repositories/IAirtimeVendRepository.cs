using BankOfChocboy.Application.DTOs;

namespace BankOfChocboy.Infrastructure.Repositories
{
    public interface IAirtimeVendRepository
    {
        Task<AirtimeVendResult> RecordAsync(AirtimeVendRequestDto request);
    }
}
