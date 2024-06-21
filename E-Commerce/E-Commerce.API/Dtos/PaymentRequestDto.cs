namespace E_Commerce.API.Dtos
{
    public class PaymentRequestDto
    {
        public string TxRef { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "NGN";
        public string RedirectUrl { get; set; }
        public string PaymentType { get; set; } = "card";
        public CustomerDto Customer { get; set; }
    }
}
