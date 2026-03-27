using Ardalis.GuardClauses;
using ErrorOr;
using Talabat.SharedKernal;

namespace Talabat.Products.Domain;

internal class Product : Entity
{
    public Guid MerchantId { get; init; }
    public string Title { get; private set; } = string.Empty;
    public decimal BasePrice { get; private set; }
    public Stock Stock { get; private set; }

    internal Product(Guid merchantId, string title, decimal basePrice, int availableStock = 0, Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
        Title = Guard.Against.NullOrEmpty(title, nameof(title));
        BasePrice = Guard.Against.NegativeOrZero(basePrice);
        Stock = Stock.Create(Guard.Against.Negative(availableStock));
    }

    public ErrorOr<Success> ReserveStock(int quantity) => Stock.Reserve(quantity);
    public ErrorOr<Success> ReleaseStock(int quantity) => Stock.Release(quantity);
    public ErrorOr<Success> DeductStock(int quantity) => Stock.Deduct(quantity);

    private Product()
    {
        // EF 
    }
}