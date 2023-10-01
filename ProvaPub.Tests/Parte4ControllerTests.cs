using Bogus;
using Castle.Core.Resource;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MockQueryable.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using ProvaPub.Controllers;
using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services;

namespace ProvaPub.Tests
{
    public class Parte4ControllerTests
    {
        private Parte4Controller _parte4Controller;

        public Parte4ControllerTests()
        {
            var mockDbContext = createMockDbContext();
            var orderService = new OrderService(mockDbContext.Object);
            var customerService = new CustomerService(mockDbContext.Object);
            this._parte4Controller = new Parte4Controller(orderService, customerService);
        }

        [Theory(DisplayName = "If customer is negative, it should throw exception")]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-37)]
        public async void IfCustomerIsNegative_ItShouldThrowException(int InvalidCustomerId)
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await this._parte4Controller.CanPurchase(InvalidCustomerId, 10m));
        }

        [Theory(DisplayName = "If customer not exists, it should throw exception")]
        [InlineData(5)]
        public async void IfCustomerNotExists_ItShouldThrowException(int NotCustomerId)
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this._parte4Controller.CanPurchase(NotCustomerId, 50));
        }

        [Theory(DisplayName = "If purchaseValue is negative, it should throw exception")]
        [InlineData(0)]
        [InlineData(-150)]
        [InlineData(-310)]
        public async void IfValueISNegative_ItShouldThrowException(int PurchaseValueNegative)
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await this._parte4Controller.CanPurchase(1, PurchaseValueNegative));
        }

        [Theory(DisplayName = "If already purchase in this month, it should returns false")]
        [InlineData(3)]
        public async void IfAlreadyPurchaseInThisMonth_ItShouldReturnsFalse(int customerId)
        {
            Assert.False(await this._parte4Controller.CanPurchase(customerId, 50));
        }

        [Theory(DisplayName = "If not purchase yet in this month, it should returns true")]
        [InlineData(1)]
        [InlineData(2)]
        public async void IfNotPurchaseYetInThisMonth_ItShouldReturnsTrue(int customerId)
        {
            Assert.True(await this._parte4Controller.CanPurchase(customerId, 50));
        }

        [Fact(DisplayName = "If never have purchase and value greater than 100, it should returns false")]
        public async void IfNeverPurchaseAndValueGreater100_ItShouldReturnsFalse()
        {
            Assert.False(await this._parte4Controller.CanPurchase(4, 150));
        }

        [Fact(DisplayName = "If never have purchase and value lower than 100, it should returns true")]
        public async void IfNeverPurchaseAndValueLower100_ItShouldReturnsTrue()
        {
            Assert.True(await this._parte4Controller.CanPurchase(4, 50));
        }

        public List<Order> GenerateOrders()
        {
            return new List<Order>
            {
                new Order() { Id = 1, OrderDate = new DateTime(2023, 05, 10), CustomerId = 1, Value = 70 },
                new Order() { Id = 2, OrderDate = new DateTime(2023, 06, 21), CustomerId = 1, Value = 250 },
                new Order() { Id = 3, OrderDate = new DateTime(2023, 07, 02), CustomerId = 2, Value = 15 },
                new Order() { Id = 4, OrderDate = new DateTime(2023, 08, 04), CustomerId = 2, Value = 135 },
                new Order() { Id = 5, OrderDate = new DateTime(2023, 03, 07), CustomerId = 3, Value = 92 },
                new Order() { Id = 6, OrderDate = DateTime.Now, CustomerId = 3, Value = 92 }
            };
        }

        public List<Customer> GenerateCustomers(List<Order> orders)
        {
            return new List<Customer>
            {
                new Customer() { Id = 1, Name = "Tanjiro", Orders = orders.Where(o => o.CustomerId == 1).ToList() },
                new Customer() { Id = 2, Name = "Nezuko", Orders = orders.Where(o => o.CustomerId == 2).ToList() },
                new Customer() { Id = 3, Name = "Zenitsu", Orders = orders.Where(o => o.CustomerId == 3).ToList() },
                new Customer() { Id = 4, Name = "Inosuke", Orders = orders.Where(o => o.CustomerId == 4).ToList() }
            };
        }

        public Mock<TestDbContext> createMockDbContext()
        {
            var orders = GenerateOrders();
            var mockOrderDbSet = orders.AsQueryable().BuildMockDbSet();
            
            var customers = GenerateCustomers(orders);
            
            var mockCustomerDbSet = customers.AsQueryable().BuildMockDbSet();
            
            var mockDbContext = new Mock<TestDbContext>();
            mockDbContext.Setup(p => p.Orders).Returns(mockOrderDbSet.Object);
            mockDbContext.Setup(p => p.Customers).Returns(mockCustomerDbSet.Object);
            mockDbContext.Setup(p => p.SaveChanges()).Returns(1);

            return mockDbContext;
        }
    }
}