namespace BankOfChocboy.Application.Interfaces
{
    public interface IDataPurchaseRepository
    {
        Task<bool> PurchaseAsync(
            string accountNumber,
            string phoneNumber,
            string network,
            string planCode,
            decimal amount,
            string reference);
    }
}
