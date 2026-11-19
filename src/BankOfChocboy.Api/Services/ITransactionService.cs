using BankOfChocboy.Application.DTOs;

namespace  BankOfChocboy.Services
{
    public interface ITransactionService
    {
        Task<List<TransactionHistoryDto>> GetTransactionHistoryAsync(
            string accountNumber);
        Task<IEnumerable<TransactionResponseDto>> FilterTransactionsAsync(string accountNumber, TransactionFilterDto filter);
    }
}