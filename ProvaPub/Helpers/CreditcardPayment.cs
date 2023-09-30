namespace ProvaPub.Helpers
{
    public class CreditcardPayment : IPaymentMethod
    {
        public decimal CalculateValue(decimal paymentValue)
        {
            return paymentValue * 1.05m;
        }
    }
}
