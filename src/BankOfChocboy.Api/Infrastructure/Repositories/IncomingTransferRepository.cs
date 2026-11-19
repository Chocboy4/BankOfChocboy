using BankOfChocboy.Infrastructure.Repositories;
using Dapper;
using MySqlConnector;
using System.Data;
using BankOfChocboy.DTOs;
using static BankOfChocboy.Infrastructure.Repositories.IIncomingTransferRepository;

namespace  BankOfChocboy.Infrastructure.Repositories
{
    public class IncomingTransferRepository : IIncomingTransferRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public IncomingTransferRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IncomingResult> CreditAsync(InterbankTransferRequestDto request)
        {
            using var connection = _connectionFactory.CreateConnection();
            if (connection.State != ConnectionState.Open) connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 1. Record the reference. A repeat of the same reference hits the UNIQUE key.
                try
                {
                    await connection.ExecuteAsync(
                        @"INSERT INTO IncomingTransfers
                          (Reference, SenderBankCode, SenderAccountNumber, RecipientAccountNumber, Amount)
                          VALUES
                          (@Reference, @SenderBankCode, @SenderAccountNumber, @RecipientAccountNumber, @Amount);",
                        request, transaction);
                }
                catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                {
                    transaction.Rollback();
                    return IncomingResult.Duplicate;
                }

                // 2. Credit the recipient. A frozen account can still receive.
                var credited = await connection.ExecuteAsync(
                    @"UPDATE Accounts SET Balance = Balance + @Amount
                      WHERE AccountNumber = @RecipientAccountNumber;",
                    request, transaction);

                if (credited != 1)
                {
                    transaction.Rollback();
                    return IncomingResult.AccountNotFound;
                }

                // 3. Show it in the recipient's history.
                await connection.ExecuteAsync(
                    @"INSERT INTO Transactions
                      (Reference, SenderAccountNumber, RecipientAccountNumber, Amount,
                       TransactionType, TransactionCategory, Description, Status, AccountNumber)
                      VALUES
                      (@Reference, @SenderAccountNumber, @RecipientAccountNumber, @Amount,
                       'Credit', 'Transfer', @Description, 'Successful', @RecipientAccountNumber);",
                    new
                    {
                        request.Reference,
                        request.SenderAccountNumber,
                        request.RecipientAccountNumber,
                        request.Amount,
                        Description = $"Transfer from {request.SenderAccountNumber} at {request.SenderBankCode}"
                    },
                    transaction);

                transaction.Commit();
                return IncomingResult.Credited;
            }
            catch
            {
                try { transaction.Rollback(); } catch { }
                throw;
            }
        }
    }
}