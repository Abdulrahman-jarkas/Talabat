namespace Talabat.Users.UnitTests.TestConstants;

public static partial class Constants
{
    public static class CheckoutSession
    {
        public static readonly Guid CustomerId = Customer.Id;
        public static readonly Guid ShopId = Shop.Id;
        public static readonly Guid AddressId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        public static readonly Guid OrderId = Guid.Parse("55555555-5555-5555-5555-555555555555");
    }
}
