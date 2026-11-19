using BankOfChocboy.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BankOfChocboy.Application.DTOs;

namespace BankOfChocboy.Controllers
{
    [Route("api/incoming-transfers")]
    [ApiController]
    [AllowAnonymous]
    [ServiceFilter(typeof(Filters.InterbankApiKeyFilter))]
    public class IncomingTransfersController : ControllerBase
    {
        private static readonly HashSet<string> KnownBanks =
            new(StringComparer.OrdinalIgnoreCase) { "TatumBank" };

        private readonly IIncomingTransferRepository _repository;

        public IncomingTransfersController(IIncomingTransferRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Receive([FromBody] InterbankTransferRequestDto request)
        {
            request.Reference = request.Reference?.Trim() ?? string.Empty;
            request.SenderAccountNumber = request.SenderAccountNumber?.Trim() ?? string.Empty;
            request.RecipientAccountNumber = request.RecipientAccountNumber?.Trim() ?? string.Empty;
            request.SenderBankCode = request.SenderBankCode?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(request.Reference) ||
                string.IsNullOrEmpty(request.SenderAccountNumber) ||
                string.IsNullOrEmpty(request.RecipientAccountNumber))
            {
                return BadRequest(new { status = "Failed", message = "Reference and account numbers are required." });
            }

            if (request.Reference.Length > 60 ||
                request.SenderAccountNumber.Length > 20 ||
                request.RecipientAccountNumber.Length > 20)
            {
                return BadRequest(new { status = "Failed", message = "A field is too long." });
            }

            // The sending bank enforces its own minimum. The receiver only rejects nonsense.
            if (request.Amount <= 0)
            {
                return BadRequest(new { status = "Failed", message = "Amount must be greater than zero." });
            }

            if (!KnownBanks.Contains(request.SenderBankCode))
            {
                return BadRequest(new { status = "Failed", message = "Unknown sender bank." });
            }

            var result = await _repository.CreditAsync(request);

            return result switch
            {
                IIncomingTransferRepository.IncomingResult.Credited =>
                    Ok(new { status = "Successful", message = "Transfer credited.", reference = request.Reference }),

                
                IIncomingTransferRepository.IncomingResult.Duplicate =>
                    Ok(new { status = "Successful", message = "Transfer already processed.", reference = request.Reference }),

                _ =>
                    NotFound(new { status = "Failed", message = "Recipient account was not found." })
            };
        }
    }
}