using BankOfChocboy.DTOs;

namespace  BankOfChocboy.Services
{
    public interface IDataPurchaseService
    {
        Task<DataPurchaseResponseDto> PurchaseDataAsync(string accountNumber, DataPurchaseRequestDto request);
        IEnumerable<NetworkPlansDto> GetAvailableDataPlans();
    }
}
