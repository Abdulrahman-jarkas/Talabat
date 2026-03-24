using ErrorOr;
using Talabat.Users.Application.Services;
using Talabat.Users.Domain.CustomerAggregate.Cart;
using CartEntity = Talabat.Users.Domain.CustomerAggregate.Cart.Cart;

namespace Talabat.Users.Domain.CustomerAggregate.Checkout;

internal interface ICheckoutSessionFactory
{
	Task<ErrorOr<CheckoutSession>> CreateCheckoutSessionAsync(
		Guid userId,
		CartEntity cart,
		IProductService productService,
		CancellationToken cancellationToken = default);
}

internal class CheckoutSessionFactory : ICheckoutSessionFactory
{
	public async Task<ErrorOr<CheckoutSession>> CreateCheckoutSessionAsync(
		Guid userId,
		CartEntity cart,
		IProductService productService,
		CancellationToken cancellationToken = default)
	{
		var productIds = cart.Items.Select(i => i.ProductId).ToList();
		var productsResult = await productService.GetProductsDetailsAsync(productIds, cancellationToken);

		if (productsResult is null || !productsResult.Any())
			return CartErrors.NoProductsFoundForCartItems;

		var checkoutItems = new List<CheckoutItem>();
		foreach (var cartItem in cart.Items)
		{
			var product = productsResult.FirstOrDefault(p => p.Id == cartItem.ProductId);
			if (product is null)
				return CartErrors.NoProductFoundForCartItem(cartItem.ProductId);

			checkoutItems.Add(CheckoutItem.Create(
				cartItem.ProductId,
				cartItem.Quantity,
				product.BasePrice));
		}

		return CheckoutSession.Create(cart.MerchantId, checkoutItems);
	}
}
