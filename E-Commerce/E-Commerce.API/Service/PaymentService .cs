using E_Commerce.API.Dtos;
using E_Commerce.API.Interfaces;

namespace E_Commerce.API.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IFlutterwavePaymentRepository _flutterwavePaymentRepository;

        public PaymentService(IFlutterwavePaymentRepository flutterwavePaymentRepository)
        {
            _flutterwavePaymentRepository = flutterwavePaymentRepository;
        }

        public async Task<string> ProcessPayment(PaymentRequestDto request)
        {
            return await _flutterwavePaymentRepository.InitializeTransaction(request);
        }

        public async Task<string> VerifyPayment(string txRef)
        {
            return await _flutterwavePaymentRepository.VerifyTransaction(txRef);
        }
    }
}
