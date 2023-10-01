using Bogus;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using ProvaPub.Controllers;
using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services;

namespace ProvaPub.Tests
{
    public class Parte4ControllerTests
    {
        private Mock<TestDbContext> _mockDbContext;
        private OrderService _orderService;
        private CustomerService _customerService;

        public Parte4ControllerTests()
        {
            this._mockDbContext = createMockDbContext();
            this._orderService = new OrderService(_mockDbContext.Object);
            this._customerService = new CustomerService(_mockDbContext.Object);
        }

        [Theory(DisplayName = "If customer is negative, it should throw exception")]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-37)]
        public async void IfCustomerIsNegative_ItShouldThrowException(int NotCustomerId)
        {
            var parte4Controller = new Parte4Controller(_orderService, _customerService);
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await parte4Controller.CanPurchase(NotCustomerId, 10m));
        }

        [Theory(DisplayName = "If purchaseValue is negative, it should throw exception")]
        [InlineData(0)]
        [InlineData(-150)]
        [InlineData(-310)]
        public async void IfValueISNegative_ItShouldThrowException(int PurchaseValueNegative)
        {
            var parte4Controller = new Parte4Controller(_orderService, _customerService);
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await parte4Controller.CanPurchase(1, PurchaseValueNegative));
        }

        [Theory(DisplayName = "If already purchase in this month, it should returns false")]
        [InlineData(3)]
        [InlineData(4)]
        public async void IfAlreadyPurchaseInThisMonth_ItShouldReturnsFalse(int customerId)
        {
            var parte4Controller = new Parte4Controller(_orderService, _customerService);
            Assert.False(await parte4Controller.CanPurchase(customerId, 50));
        }

        [Theory(DisplayName = "If not purchase yet in this month, it should returns true")]
        [InlineData(1)]
        [InlineData(2)]
        public async void IfNotPurchaseYetInThisMonth_ItShouldReturnsTrue(int customerId)
        {
            var parte4Controller = new Parte4Controller(_orderService, _customerService);
            Assert.True(await parte4Controller.CanPurchase(customerId, 50));
        }

        [Fact(DisplayName = "If never have purchase and value greater than 100, it should returns false")]
        public async void IfNeverPurchaseAndValueGreater100_ItShouldReturnsFalse()
        {
            var parte4Controller = new Parte4Controller(_orderService, _customerService);
            Assert.False(await parte4Controller.CanPurchase(4, 150));
        }

        [Fact(DisplayName = "If never have purchase and value lower than 100, it should returns false")]
        public async void IfNeverPurchaseAndValueLower100_ItShouldReturnsTrue()
        {
            var parte4Controller = new Parte4Controller(_orderService, _customerService);
            Assert.False(await parte4Controller.CanPurchase(4, 50));
        }

        public IQueryable<Order> GenerateOrders()
        {
            var list = new List<Order>
            {
                new Order() { Id = 1, OrderDate = new DateTime(2023, 05, 10), CustomerId = 1, Value = 70 },
                new Order() { Id = 2, OrderDate = new DateTime(2023, 06, 21), CustomerId = 1, Value = 250 },
                new Order() { Id = 3, OrderDate = new DateTime(2023, 07, 02), CustomerId = 2, Value = 15 },
                new Order() { Id = 4, OrderDate = new DateTime(2023, 08, 04), CustomerId = 2, Value = 135 },
                new Order() { Id = 5, OrderDate = new DateTime(2023, 03, 07), CustomerId = 3, Value = 92 },
                new Order() { Id = 6, OrderDate = new DateTime(), CustomerId = 3, Value = 92 }
            };
            return list.AsQueryable();
        }

        public IQueryable<Customer> GenerateCustomers()
        {
            var list = new List<Customer>
            {
                new Customer() { Id = 1, Name = "Tanjiro" },
                new Customer() { Id = 2, Name = "Nezuko" },
                new Customer() { Id = 3, Name = "Zenitsu" },
                new Customer() { Id = 4, Name = "Inosuke" }
            };
            return list.AsQueryable();
        }

        public Mock<TestDbContext> createMockDbContext()
        {
            var orders = GenerateOrders();
            var mockOrderDbSet = new Mock<DbSet<Order>>();
            mockOrderDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            mockOrderDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            mockOrderDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            mockOrderDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(() => orders.GetEnumerator());

            var customer = GenerateCustomers();
            var mockCustomerDbSet = new Mock<DbSet<Customer>>();
            mockCustomerDbSet.As<IQueryable<Customer>>().Setup(m => m.Provider).Returns(customer.Provider);
            mockCustomerDbSet.As<IQueryable<Customer>>().Setup(m => m.ElementType).Returns(customer.ElementType);
            mockCustomerDbSet.As<IQueryable<Customer>>().Setup(m => m.Expression).Returns(customer.Expression);
            mockCustomerDbSet.As<IQueryable<Customer>>().Setup(m => m.GetEnumerator()).Returns(() => customer.GetEnumerator());

            var mockDbContext = new Mock<TestDbContext>();
            mockDbContext.Setup(p => p.Orders).Returns(mockOrderDbSet.Object);
            mockDbContext.Setup(p => p.Customers).Returns(mockCustomerDbSet.Object);
            mockDbContext.Setup(p => p.SaveChanges()).Returns(1);

            return mockDbContext;
        }
    }
}