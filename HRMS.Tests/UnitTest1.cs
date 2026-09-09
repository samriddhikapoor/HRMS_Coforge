using Xunit;

namespace HRMS.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            // Arrange
            int a = 10;
            int b = 20;

            // Act
            int result = a + b;

            // Assert
            Assert.Equal(30, result);
        }
    }
}