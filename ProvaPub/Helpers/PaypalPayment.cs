namespace ProvaPub.Helpers
{
    public class PaypalPayment : IPaymentMethod
    {
        public decimal CalculateValue(decimal paymentValue)
        {
            return paymentValue * 0.95m;
        }
    }
}
