using ErrorOr;
using MediatR;
using Talabat.Products.Contracts;
using Talabat.Users.Contracts;

namespace Talabat.Invoices;

internal class InvoicesService(ISender sender, IInvoicesRepository invoicesRepository) : IInvoicesService
{
	public async Task<ErrorOr<Invoice>> CreateInvoiceAsync()
	{
		// extract user id from the token 
		// @TODO: replace with actual user id
		var userId = Guid.Parse("1fb673f4-6974-478b-b4eb-b9882dd13c5f");
		var customer = await invoicesRepository.GetCustomerAsync(userId);

		if (customer is null)
			return Error.NotFound("User.NotFound", "The user was not found.");

		// get cart items 
		//@TODO: is this the best way to get cart details?
		var cart = await sender.Send(new CartDetailsQuery(userId));

		if (cart is null || !cart.CartItems.Any())
			return Error.Failure("Cart.Empty", "The cart is empty.");

		// get all products details from products module
		//@TODO: is this the best way to get products details?
		var productIds = cart.CartItems.Select(ci => ci.productId).ToList();
		var productsResult = await sender.Send(new ProductsQuery(productIds));

		if (productsResult is null || !productsResult.Any())
			return Error.Failure("Products.NotFound", "No products were found for the given cart items.");

		// add invoice
		// @TODO: how we make sure that invoice has valid state, we now just retue
		// @TODO: invoice is entity inside customer aggregate root, so we need to make sure that we are following DDD principles
		var invoiceItems = new List<InvoiceItem>();
		foreach (var cartItem in cart.CartItems)
		{
			var product = productsResult.FirstOrDefault(p => p.Id == cartItem.productId);
			if (product is null)
				return Error.Failure("Product.NotFound", $"Product with id {cartItem.productId} was not found.");

			invoiceItems.Add(InvoiceItem.Create(
				product.Id,
				cartItem.quantity,
				product.BasePrice));
		}

		var invoice = new Invoice(userId, cart.MerchantId, invoiceItems);

		var addInvoiceResult = customer.AddInvoice(invoice);

		if (addInvoiceResult.IsError)
			return addInvoiceResult.Errors;

		await invoicesRepository.SaveChangesAsync();

		return invoice;
	}
}