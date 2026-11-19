using BankOfChocboy.DTOs;
using BankOfChocboy.Helpers;
using BankOfChocboy.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankOfChocboy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [ServiceFilter(typeof(Filters.InterbankApiKeyFilter))]
    public class AirtimeVendController : ControllerBase
    {
        private static readonly HashSet<string> KnownBanks =
            new(StringComparer.OrdinalIgnoreCase) { "TatumBank" };

        private readonly IAirtimeVendRepository _repository;

        public AirtimeVendController(IAirtimeVendRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Vend([FromBody] AirtimeVendRequestDto request)
        {
            request.Reference = request.Reference?.Trim() ?? string.Empty;
            request.RequesterBankCode = request.RequesterBankCode?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(request.Reference) || request.Reference.Length > 60 ||
                string.IsNullOrEmpty(request.RequesterBankCode))
            {
                return BadRequest(new { status = "Failed", message = "Reference and requester bank code are required." });
            }

            if (!KnownBanks.Contains(request.RequesterBankCode))
            {
                return BadRequest(new { status = "Failed", message = "Unknown requester bank." });
            }

            if (request.Amount < 50 || request.Amount > 100000)
            {
                return BadRequest(new { status = "Failed", message = "Amount must be between 50 and 100000." });
            }

            var network = NetworkCatalog.Normalize(request.Network);

            if (!NetworkCatalog.IsSupported(network))
            {
                return BadRequest(new
                {
                    status = "Failed",
                    message = "Invalid network."
                });
            }

            if (!PhoneNumberHelper.TryNormalize(request.PhoneNumber, out var phoneNumber))
            {
                return BadRequest(new { status = "Failed", message = "Invalid phone number." });
            }
            request.PhoneNumber = phoneNumber;
            request.Network = network;

            var result = await _repository.RecordAsync(request);

            return result switch
            {
                AirtimeVendResult.Recorded => Ok(new { status = "Success", message = "Airtime vend request recorded successfully." }),
                _ => Ok(new { status = "Successful", message = "Airtime already processed.", reference = request.Reference })
            };
        }
    }
}
