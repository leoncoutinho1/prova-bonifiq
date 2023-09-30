using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ProvaPub.Helpers;
using ProvaPub.Models;

namespace ProvaPub.Services
{
	public class OrderService
	{
		DbContext _ctx;
		public OrderService(DbContext ctx) 
		{
			_ctx = ctx;
		}
		public async Task<Order> PayOrder(string paymentMethod, decimal paymentValue, int customerId)
		{
			var payment = PaymentMethodFactory.create(paymentMethod);
			paymentValue = payment.CalculateValue(paymentValue);

			return await Task.FromResult( new Order()
			{
				Value = paymentValue
			});
		}
	}
}
