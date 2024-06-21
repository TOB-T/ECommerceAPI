using E_Commerce.API.Dtos;

namespace E_Commerce.API.Interfaces
{
    public interface IPaymentService
    {
        Task<string> ProcessPayment(PaymentRequestDto request);
        Task<string> VerifyPayment(string txRef);
    }
}
