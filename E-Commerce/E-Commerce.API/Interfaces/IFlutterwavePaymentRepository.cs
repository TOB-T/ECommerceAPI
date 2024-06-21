using E_Commerce.API.Dtos;

namespace E_Commerce.API.Interfaces
{
    public interface IFlutterwavePaymentRepository
    {
        Task<string> InitializeTransaction(PaymentRequestDto request);
        Task<string> VerifyTransaction(string txRef);
    }
}