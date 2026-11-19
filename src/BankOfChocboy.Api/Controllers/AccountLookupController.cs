using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BankOfChocboy.Application.DTOs;
using BankOfChocboy.Infrastructure.Repositories;

namespace BankOfChocboy.Controllers
{
    [Route("api/accounts/lookup")]
    [ApiController]
    [AllowAnonymous]
    [ServiceFilter(typeof(Filters.InterbankApiKeyFilter))]
    public class AccountLookupController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        public AccountLookupController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpGet("{accountNumber}")]
        public async Task<IActionResult> LookUp(string accountNumber)
        {
            var account = await _accountRepository.GetAccountByNumberAsync(accountNumber.Trim());

            if (account == null)
            {
                return NotFound(new { status = "Failed", message = "Account not found." });
            }

            return Ok(new InterbankAccountLookupResponseDto
            {
                AccountHolderName = account.AccountHolderName,
                AccountNumber = account.AccountNumber
            });
        }
    }
}