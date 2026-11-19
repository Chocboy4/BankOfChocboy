using BankOfChocboy.Application.DTOs;

namespace BankOfChocboy.Application.Interfaces
{
    public interface IDataPurchaseService
    {
        Task<DataPurchaseResponseDto> PurchaseDataAsync(string accountNumber, DataPurchaseRequestDto request);
        IEnumerable<NetworkPlansDto> GetAvailableDataPlans();
    }
}
