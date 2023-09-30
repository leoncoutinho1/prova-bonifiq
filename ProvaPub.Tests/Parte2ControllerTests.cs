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
        private Mock<TestDbContext> mockDbContext;

        public Parte2ControllerTests()
        {
            var data = GenerateData(20).AsQueryable();

            var mockDbset = new Mock<DbSet<Product>>();
            mockDbset.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(data.Provider);
            mockDbset.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockDbset.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(data.Expression);
            mockDbset.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            this.mockDbContext = new Mock<TestDbContext>();
            this.mockDbContext.Setup(p => p.Products).Returns(mockDbset.Object);
            this.mockDbContext.Setup(p => p.SaveChanges()).Returns(1);
        }

        [Fact(DisplayName = "Getting list product, it should have 10 products")]
        public void GettingListProducts_ItShouldHave10Products()
        {
            var parte2Controller = new Parte2Controller(this.mockDbContext.Object);

            var list = parte2Controller.ListProducts(1);

            Assert.Equal(10, list.Products.Count);
        }

        [Fact(DisplayName = "Getting two different pages, they should be differents")]
        public void GettingTwoDifferentPages_TheyShouldBeDIfferents()
        {
            var parte2Controller = new Parte2Controller(this.mockDbContext.Object);

            var list1 = parte2Controller.ListProducts(1);
            var list2 = parte2Controller.ListProducts(2);
            var intersect = list1.Products.Intersect(list2.Products);
            
            Assert.False(intersect.Count() == list1.Products.Count);
        }

        public List<Product> GenerateData(int count)
        {
            var Faker = new Faker<Product>()
                .RuleFor(p => p.Id, f => f.Random.Number(1,100))
                .RuleFor(p => p.Name, f => f.Commerce.Product());

            return Faker.Generate(count);
        }
    }
}