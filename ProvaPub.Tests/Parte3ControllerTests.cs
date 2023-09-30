using Bogus;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProvaPub.Controllers;
using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services;

namespace ProvaPub.Tests
{
    public class Parte3ControllerTests
    {
        private Mock<TestDbContext> _mockDbContext;
        private OrderService _orderService;

        public Parte3ControllerTests()
        {
            var mockDbContext = new Mock<TestDbContext>();
            _orderService = new OrderService(mockDbContext.Object);
        }

        
        [Theory(DisplayName = "Getting differents payment methods, they should have differents values")]
        [InlineData("pix",138)]
        [InlineData("creditcard", 157.5)]
        [InlineData("paypal", 142.5)]
        [InlineData("", 150)]
        public async void GettingDifferentsMethods_TheyShouldHaveDifferentsValues(string paymentMethod, decimal expectedValue)
        {
            var parte3Controller = new Parte3Controller(_orderService);
            
            // valor 150 - pix tem 8% de desconto - total = 138
            Assert.Equal(expectedValue, (await _orderService.PayOrder(paymentMethod, 150, 1)).Value);
        }
    }
}