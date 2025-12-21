using ErrorOr;

namespace Talabat.Users;

internal record CartErrors
{
	public static Error CartItemNotFound
		=> Error.NotFound("CartItem.NotFound", "The cart item was not found.");

	public static Error CartNotFound
		=> Error.NotFound("Cart.NotFound", "The cart was not found.");

	public static Error NoProductFoundForCartItem(Guid productId)
		=> Error.NotFound("CartItem.ProductNotFound", $"No product was found for the cart item with product id {productId}.");

	public static Error NoProductsFoundForCartItems
				=> Error.NotFound("CartItems.ProductsNotFound", "No products were found for the cart items.");
}