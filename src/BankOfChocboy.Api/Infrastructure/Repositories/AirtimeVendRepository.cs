using BankOfChocboy.DTOs;
using Dapper;
using MySqlConnector;

namespace BankOfChocboy.Infrastructure.Repositories
{
    public class AirtimeVendRepository : IAirtimeVendRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AirtimeVendRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<AirtimeVendResult> RecordAsync(AirtimeVendRequestDto request)
        {
            var connection = _connectionFactory.CreateConnection();

            try
            {
                await connection.ExecuteAsync(
                    @"INSERT INTO AirtimeVends
                      (Reference, RequesterBankCode, PhoneNumber, Network, Amount)
                      VALUES
                      (@Reference, @RequesterBankCode, @PhoneNumber, @Network, @Amount);",
                    request);

                return AirtimeVendResult.Recorded;
            }
            catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
            {
                
                return AirtimeVendResult.Duplicate;
            }
        }
    }
}
