using E_Commerce.API.Dtos;
using E_Commerce.API.Interfaces;
using System.Net.Http.Headers;

namespace E_Commerce.API.Repositories
{
    public class FlutterwavePaymentRepository : IFlutterwavePaymentRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public FlutterwavePaymentRepository(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("Flutterwave");
            _configuration = configuration;
        }

        public async Task<string> InitializeTransaction(PaymentRequestDto request)
        {
            var url = "payments";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["Flutterwave:SecretKey"]);

            var response = await _httpClient.PostAsJsonAsync(url, request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<FlutterwaveResponseDto>();
            return result.Data.Link;
        }


        public async Task<string> VerifyTransaction(string txRef)
        {
            var url = $"transactions/verify_by_reference?tx_ref={txRef}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["Flutterwave:SecretKey"]);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<FlutterwaveResponseDto>();
            return result.Status == "success" ? "Transaction successful" : "Transaction failed";
        }
    }
}
