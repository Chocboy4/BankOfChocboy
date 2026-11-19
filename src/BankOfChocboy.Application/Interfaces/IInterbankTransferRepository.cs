namespace BankOfChocboy.Application.Interfaces
{
    public interface IInterbankTransferRepository
    {
        Task<bool> DebitAndRecordPendingAsync(
            string senderAccountNumber, string recipientAccountNumber,
            string recipientBankCode, decimal amount, string reference);

        Task MarkSuccessfulAsync(string reference);

        Task<bool> RefundAndMarkFailedAsync(string reference);
    }
}