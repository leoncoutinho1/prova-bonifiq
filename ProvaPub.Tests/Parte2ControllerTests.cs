using Bogus;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProvaPub.Controllers;
using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services;

namespace ProvaPub.Tests
{
    public class Parte2ControllerTests
    {
        private Mock<TestDbContext> _mockDbContext;
        private ProductService _productService;
        private CustomerService _customerService;

        public Parte2ControllerTests()
        {
            this._mockDbContext = createMockDbContext();
            this._productService = new ProductService(_mockDbContext.Object);
            this._customerService = new CustomerService(_mockDbContext.Object);
        }

        [Fact(DisplayName = "Getting list product, it should have 10 products")]
        public void GettingListProducts_ItShouldHave10Products()
        {
            var parte2Controller = new Parte2Controller(_productService, _customerService);

            var list = parte2Controller.ListProducts(1);

            Assert.Equal(10, list.Items.Count);
        }

        [Fact(DisplayName = "Getting two different pages, they should be differents")]
        public void GettingTwoDifferentPages_TheyShouldBeDIfferents()
        {
            var parte2Controller = new Parte2Controller(_productService, _customerService);

            var list1 = parte2Controller.ListProducts(1);
            var list2 = parte2Controller.ListProducts(2);
            var intersect = list1.Items.Intersect(list2.Items);
            
            Assert.False(intersect.Count() == list1.Items.Count);
        }

        public List<Product> GenerateProductsData(int count)
        {
            var Faker = new Faker<Product>()
                .RuleFor(p => p.Id, f => f.Random.Number(1,100))
                .RuleFor(p => p.Name, f => f.Commerce.Product());

            return Faker.Generate(count);
        }

        public List<Customer> GenerateCustomersData(int count)
        {
            var Faker = new Faker<Customer>()
                .RuleFor(p => p.Id, f => f.Random.Number(1, 100))
                .RuleFor(p => p.Name, f => f.Person.FirstName);

            return Faker.Generate(count);
        }

        public Mock<TestDbContext> createMockDbContext()
        {
            var products = GenerateProductsData(20).AsQueryable();
            var mockProductDbSet = new Mock<DbSet<Product>>();
            mockProductDbSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(products.Provider);
            mockProductDbSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.ElementType);
            mockProductDbSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.Expression);
            mockProductDbSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(() => products.GetEnumerator());

            var customer = GenerateCustomersData(20).AsQueryable();
            var mockCustomerDbSet = new Mock<DbSet<Customer>>();
            mockCustomerDbSet.As<IQueryable<Customer>>().Setup(m => m.Provider).Returns(customer.Provider);
            mockCustomerDbSet.As<IQueryable<Customer>>().Setup(m => m.ElementType).Returns(customer.ElementType);
            mockCustomerDbSet.As<IQueryable<Customer>>().Setup(m => m.Expression).Returns(customer.Expression);
            mockCustomerDbSet.As<IQueryable<Customer>>().Setup(m => m.GetEnumerator()).Returns(() => customer.GetEnumerator());

            var mockDbContext = new Mock<TestDbContext>();
            mockDbContext.Setup(p => p.Products).Returns(mockProductDbSet.Object);
            mockDbContext.Setup(p => p.Customers).Returns(mockCustomerDbSet.Object);
            mockDbContext.Setup(p => p.SaveChanges()).Returns(1);

            return mockDbContext;
        }
    }
}