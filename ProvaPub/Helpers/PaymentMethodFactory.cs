namespace ProvaPub.Helpers
{
    public static class PaymentMethodFactory
    {
        public static IPaymentMethod create(string paymentMethod)
        {
            switch (paymentMethod)
            {
                case "pix":
                        return new PixPayment();
                case "creditcard":
                    return new CreditcardPayment();
                case "paypal":
                    return new PaypalPayment();
            }

            return new DefaultPayment();
        }
    }
}
