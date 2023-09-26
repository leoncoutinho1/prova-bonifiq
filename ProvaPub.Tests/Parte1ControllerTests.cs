using Moq;
using ProvaPub.Controllers;
using ProvaPub.Services;

namespace ProvaPub.Tests
{
    public class Parte1ControllerTests
    {
        [Fact(DisplayName = "Getting two numbers from api, they should be differents")]
        public void GettingTwoNumbers_TheyShouldBeDifferents()
        {
            var mock = new Mock<RandomService>();
            var parte1Controller = new Parte1Controller(mock.Object);

            var number1 = parte1Controller.Index();
            var number2 = parte1Controller.Index();

            Assert.NotEqual(number1, number2);
        }
    }
}