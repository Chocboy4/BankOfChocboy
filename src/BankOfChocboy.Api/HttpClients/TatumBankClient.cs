using System.Net;
using System.Net.Http.Json;
using BankOfChocboy.Application.DTOs;

namespace BankOfChocboy.HttpClients
{
    public class TatumBankClient : ITatumBankClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TatumBankClient> _logger;

        public TatumBankClient(HttpClient httpClient, ILogger<TatumBankClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<InterbankResult> SendTransferAsync(
            InterbankTransferRequestDto request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(
                    "/api/incoming-transfers", request, cancellationToken);

                var status = (int)response.StatusCode;
                var body = await ReadBodyAsync(response, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return new InterbankResult(InterbankOutcome.Accepted,
                        body?.Message ?? "Transfer accepted by TatumBank.");
                }

                _logger.LogWarning(
                    "TatumBank answered {Status} for {Reference} at {Url}. Message: {Message}",
                    status, request.Reference, response.RequestMessage?.RequestUri, body?.Message);

                if (status >= 500 || status == 408 || status == 429)
                {
                    return new InterbankResult(InterbankOutcome.Unknown,
                        $"TatumBank returned status {status}.");
                }

                return new InterbankResult(InterbankOutcome.Rejected,
                    body?.Message ?? $"TatumBank rejected the transfer (status {status}).");
            }
            catch (HttpRequestException ex) when (ex.HttpRequestError == HttpRequestError.ConnectionError)
            {
                _logger.LogWarning(ex, "Could not connect to TatumBank for {Reference}.", request.Reference);
                return new InterbankResult(InterbankOutcome.NotSent, "TatumBank is currently unavailable.");
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                _logger.LogWarning(ex, "No clear answer from TatumBank for {Reference}.", request.Reference);
                return new InterbankResult(InterbankOutcome.Unknown, "No confirmation received from TatumBank.");
            }
        }

        public async Task<AccountLookupResult> LookupAccountAsync(
            string accountNumber,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var response = await _httpClient.GetAsync(
                    $"/api/accounts/lookup/{Uri.EscapeDataString(accountNumber)}",
                    cancellationToken);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("TatumBank lookup returned 404 for {Url}.",
                        response.RequestMessage?.RequestUri);

                    return new AccountLookupResult(AccountLookupOutcome.NotFound, null,
                        "Account not found.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("TatumBank lookup returned {Status} for {Url}.",
                        (int)response.StatusCode, response.RequestMessage?.RequestUri);

                    return new AccountLookupResult(AccountLookupOutcome.Unavailable, null,
                        $"TatumBank returned status {(int)response.StatusCode}.");
                }

                var body = await response.Content
                    .ReadFromJsonAsync<InterbankAccountLookupResponseDto>(cancellationToken: cancellationToken);

                return new AccountLookupResult(AccountLookupOutcome.Found, body?.AccountHolderName,
                    "Account found.");
            }
            catch (Exception ex) when (
                ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
            {
                _logger.LogWarning(ex, "No clear answer from TatumBank for account lookup {AccountNumber}.",
                    accountNumber);

                return new AccountLookupResult(AccountLookupOutcome.Unavailable, null,
                    "No confirmation received from TatumBank.");
            }
        }

        private static async Task<InterbankTransferResponseDto?> ReadBodyAsync(
            HttpResponseMessage response, CancellationToken cancellationToken)
        {
            try
            {
                return await response.Content.ReadFromJsonAsync<InterbankTransferResponseDto>(
                    cancellationToken: cancellationToken);
            }
            catch
            {
                return null;
            }
        }
    }
}