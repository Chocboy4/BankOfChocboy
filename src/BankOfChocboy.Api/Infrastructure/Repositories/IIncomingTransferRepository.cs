using BankOfChocboy.Application.DTOs;

namespace BankOfChocboy.Infrastructure.Repositories
{
    public interface IIncomingTransferRepository
    {
        public enum IncomingResult
        {
            Credited,
            Duplicate,
            AccountNotFound
        }
        Task<IncomingResult> CreditAsync(InterbankTransferRequestDto request);
    }
}
