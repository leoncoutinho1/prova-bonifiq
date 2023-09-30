namespace ProvaPub.Helpers
{
    public interface IPaymentMethod
    {
        decimal CalculateValue(decimal paymentValue);
    }
}
