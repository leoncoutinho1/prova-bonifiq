namespace ProvaPub.Helpers
{
    public class DefaultPayment : IPaymentMethod
    {
        public decimal CalculateValue(decimal paymentValue)
        {
            return paymentValue;
        }
    }
}
