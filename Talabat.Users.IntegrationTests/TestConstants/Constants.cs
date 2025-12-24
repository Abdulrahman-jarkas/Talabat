namespace Talabat.Users.IntegrationTests.TestConstants;

public static class Constants
{
    public static class Customer
    {
        public static readonly Guid Id = Guid.Parse("1fb673f4-6974-478b-b4eb-b9882dd13c5f");
        public static readonly string Email = "customer@test.com";
    }

    public static class Merchant
    {
        public static readonly Guid Id = Guid.Parse("1fb673f4-6974-478b-b4eb-b9882dd13c5c");
        public static readonly string Email = "merchant@test.com";
    }

    public static class Product
    {
        public static readonly Guid Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly Guid AlternativeId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        public static readonly string Title = "Test Product";
        public static readonly decimal BasePrice = 100.00m;
        public static readonly int DefaultQuantity = 2;
        public static readonly int UpdatedQuantity = 5;
    }

    public static class Address
    {
        public static readonly string DefaultAddress = "123 Test Street, Test City";
        public static readonly string AlternativeAddress = "456 Another Street, Another City";
    }

    public static class Payment
    {
        public static readonly Guid PaymentId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        public static readonly string PaymentUrl = "https://payment.test.com/pay/123";
    }

    public static class CheckoutSession
    {
        public static readonly decimal TotalPrice = 200.00m;
    }
}
