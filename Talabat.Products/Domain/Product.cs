using Ardalis.GuardClauses;
using Talabat.SharedKernal;

namespace Talabat.Products.Domain;

internal class Product : Entity
{
    public Guid MerchantId { get; init; }
    public string Title { get; private set; } = string.Empty;
    public decimal BasePrice { get; private set; }

    internal Product(Guid merchantId, string title, decimal basePrice, Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
        Title = Guard.Against.NullOrEmpty(title, nameof(title));
        BasePrice = Guard.Against.NegativeOrZero(basePrice);
    }

    private Product()
    {
        // EF 
    }
}