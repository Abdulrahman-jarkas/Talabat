namespace Talabat.Users.UnitTests.TestConstants;

public static partial class Constants
{
    public static class Product
    {
        public static readonly Guid Id = Guid.Parse("66666666-6666-6666-6666-666666666666");
        public static readonly Guid AlternativeId = Guid.Parse("77777777-7777-7777-7777-777777777777");
        public static readonly int DefaultQuantity = 2;
        public static readonly int UpdatedQuantity = 5;
        public static readonly decimal DefaultPrice = 99.99m;
    }
}
