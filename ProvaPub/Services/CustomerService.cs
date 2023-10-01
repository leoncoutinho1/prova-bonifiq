using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public class CustomerService
    {
        TestDbContext _ctx;

        public CustomerService(TestDbContext ctx)
        {
            _ctx = ctx;
        }

        public CustomList<Customer> ListCustomers(int page)
        {
            return new CustomList<Customer>() { HasNext = false, TotalCount = 10, Items = _ctx.Customers.ToList() };
        }

        public async Task<bool> CanPurchase(int customerId, decimal purchaseValue)
        {
            if (customerId <= 0) throw new ArgumentOutOfRangeException(nameof(customerId));

            if (purchaseValue <= 0) throw new ArgumentOutOfRangeException(nameof(purchaseValue));

            //Business Rule: Non registered Customers cannot purchase
            var customer = await _ctx.Customers.Where(c => c.Id == customerId).SingleOrDefaultAsync();
            if (customer == null) throw new InvalidOperationException($"Customer Id {customerId} does not exists");

            //Business Rule: A customer can purchase only a single time per month
            var baseDate = DateTime.UtcNow.AddMonths(-1);
            var ordersInThisMonth = await _ctx.Orders.Where(s => s.CustomerId == customerId && s.OrderDate >= baseDate).CountAsync();
            if (ordersInThisMonth > 0)
                return false;

            //Business Rule: A customer that never bought before can make a first purchase of maximum 100,00
            var haveBoughtBefore = await _ctx.Customers.Where(s => s.Id == customerId && s.Orders.Any()).CountAsync();
            if (haveBoughtBefore == 0 && purchaseValue > 100)
                return false;

            // O Moq não estava funcionando bem com chamadas assíncronas então tive que alterar este método.
            // Troquei as chamadas async por Where e chamei logo em seguida a função async

            return true;
        }

    }
}
