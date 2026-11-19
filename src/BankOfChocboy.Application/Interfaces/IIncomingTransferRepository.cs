using BankOfChocboy.Application.DTOs;

namespace BankOfChocboy.Application.Interfaces
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
