using Ardalis.GuardClauses;
using ErrorOr;
using Talabat.Products.Domain.Events;
using Talabat.SharedKernal;

namespace Talabat.Products.Domain;

internal class Product : AggregateRoot
{
    public Guid ShopId { get; init; }
    public string Title { get; private set; } = string.Empty;
    public decimal BasePrice { get; private set; }
    public Stock Stock { get; private set; }
    public bool IsDeleted { get; private set; }

    internal Product(Guid shopId, string title, decimal basePrice, int quantity = 0, Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        ShopId = Guard.Against.Default(shopId, nameof(shopId));
        Title = Guard.Against.NullOrEmpty(title, nameof(title));
        BasePrice = Guard.Against.NegativeOrZero(basePrice);
        Stock = Stock.Create(quantity);
        IsDeleted = false;

        _domainEvents.Add(new ProductCreatedEvent(Id, BasePrice, quantity));
    }

    public ErrorOr<Success> AddReservation(Guid checkoutSessionId, Guid userId, int quantity)
    {
        var result = Stock.AddReservation(checkoutSessionId, userId, quantity);
        if (result.IsError)
            return result.Errors;

        Stock = result.Value;
        _domainEvents.Add(new ProductQuantityChangedEvent(Id, Stock.EffectiveQuantity));
        return Result.Success;
    }

    public ErrorOr<Success> EditReservation(Guid checkoutSessionId, Guid orderId)
    {
        var result = Stock.SetOrderId(checkoutSessionId, orderId);
        if (result.IsError)
            return result.Errors;

        Stock = result.Value;
        return Result.Success;
    }

    public ErrorOr<Success> RemoveReservation(Guid checkoutSessionId)
    {
        var result = Stock.RemoveReservation(checkoutSessionId);
        if (result.IsError)
            return result.Errors;

        Stock = result.Value;
        _domainEvents.Add(new ProductQuantityChangedEvent(Id, Stock.EffectiveQuantity));
        return Result.Success;
    }

    public ErrorOr<Success> ConfirmShipment(Guid orderId)
    {
        var result = Stock.DeductShipped(orderId);
        if (result.IsError)
            return result.Errors;

        Stock = result.Value;
        _domainEvents.Add(new ProductQuantityChangedEvent(Id, Stock.EffectiveQuantity));
        return Result.Success;
    }

    public ErrorOr<Success> UpdateQuantity(int newQuantity)
    {
        var result = Stock.WithUpdatedQuantity(newQuantity);
        if (result.IsError)
            return result.Errors;

        Stock = result.Value;
        _domainEvents.Add(new ProductQuantityChangedEvent(Id, Stock.EffectiveQuantity));
        return Result.Success;
    }

    public ErrorOr<Success> UpdatePrice(decimal newPrice)
    {
        BasePrice = Guard.Against.NegativeOrZero(newPrice);
        Stock = Stock.RemoveAllUnpaidReservations();
        _domainEvents.Add(new ProductPriceChangedEvent(Id, BasePrice));
        _domainEvents.Add(new ProductQuantityChangedEvent(Id, Stock.EffectiveQuantity));
        return Result.Success;
    }

	public ErrorOr<Success> SoftDelete()
	{
		if (Stock.HasPaidReservations)
			return ProductErrors.HasPaidReservations(Id);

		IsDeleted = true;
		Stock = Stock.RemoveAllUnpaidReservations();
		_domainEvents.Add(new ProductSoftDeletedEvent(Id));
		return Result.Success;
	}

    private Product()
    {
        // EF 
    }
}