using BankOfChocboy.Application.DTOs;
using BankOfChocboy.Domain.Models;

namespace BankOfChocboy.Application.Interfaces
{
    public interface ITransactionRepository
    {
        Task<bool> AddAsync(Transfer transfer);

        Task<List<Transaction>> GetByAccountNumberAsync(
            string accountNumber);

        Task<IEnumerable<TransactionResponseDto>> FilterAsync(string accountNumber, TransactionFilterDto filter);
    }
}