using FluentAssertions;
using Talabat.Users.UnitTests.TestConstants;
using Talabat.Users.UnitTests.TestUtils.Factories;

namespace Talabat.Users.UnitTests.MerchantAggregate;

public class MerchantTests
{
    [Fact]
    public void Create_WithValidEmail_ShouldCreateMerchant()
    {
        // Arrange
        var email = Constants.Merchant.Email;
        var id = Constants.Merchant.Id;

        // Act
        var merchant = MerchantFactory.Create(email, id);

        // Assert
        merchant.Should().NotBeNull();
        merchant.Email.Should().Be(email);
        merchant.Id.Should().Be(id);
    }
}
