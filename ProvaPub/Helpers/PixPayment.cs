namespace ProvaPub.Helpers
{
    public class PixPayment : IPaymentMethod
    {
        public decimal CalculateValue(decimal paymentValue)
        {
            return paymentValue * 0.92m;
        }
    }
}
