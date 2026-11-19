using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Cryptography;
using System.Text;

namespace  BankOfChocboy.Filters
{
    public class InterbankApiKeyFilter : IActionFilter
    {
        private readonly IConfiguration _configuration;

        public InterbankApiKeyFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var expected = _configuration["Interbank:ApiKey"];

            if (string.IsNullOrEmpty(expected) ||
                !context.HttpContext.Request.Headers.TryGetValue("X-Api-Key", out var provided) ||
                !CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(provided.ToString()),
                    Encoding.UTF8.GetBytes(expected)))
            {
                context.Result = new UnauthorizedObjectResult(
                    new { status = "Failed", message = "Invalid API key." });
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}