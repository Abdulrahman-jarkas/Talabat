using ErrorOr;
using MediatR;
using Talabat.Payments.Contracts;
using Talabat.Products.Contracts;
using Talabat.Users.Data.Repositories;
using Talabat.Users.Domain.CustomerAggregate;
using Talabat.Users.Domain.CustomerAggregate.Cart;
using Talabat.Users.Domain.CustomerAggregate.Checkout;

namespace Talabat.Users;

internal class CustomerService(ISender sender, IUsersRepository usersRepository) : ICustomerService
{
    private readonly Guid customerId = Guid.Parse("1fb673f4-6974-478b-b4eb-b9882dd13c5f");

    public async Task<ErrorOr<Success>> AddCartItemAsync(Guid productId, int quantity, CancellationToken cancellationToken)
    {
        var customer = await usersRepository.GetCustomerAsync(customerId);
        if (customer is null)
            return CustomerErrors.CustomerNotFound;

        var product = await sender.Send(new ProductQuery(productId), cancellationToken);
        if (product is null)
            return CartErrors.NoProductFoundForCartItem(productId);

        var setCartResult = customer.SetCartItem(product.Merchant, productId, quantity);
        if (setCartResult.IsError)
            return setCartResult.Errors;

        await usersRepository.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }

    public async Task<ErrorOr<Success>> RemoveCartItemAsync(Guid productId, CancellationToken cancellationToken)
    {
        var customer = await usersRepository.GetCustomerAsync(customerId);
        if (customer is null)
            return CustomerErrors.CustomerNotFound;

        var setCartResult = customer.RemoveCartItem(productId);
        if (setCartResult.IsError)
            return setCartResult.Errors;

        await usersRepository.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }

    public async Task<ErrorOr<Success>> CreateCheckoutSession(CancellationToken cancellationToken = default)
    {
        var customer = await usersRepository.GetCustomerAsync(customerId);

        if (customer is null)
            return CustomerErrors.CustomerNotFound;

        if (customer.Cart is null || !customer.Cart.Items.Any())
            return CartErrors.CartNotFound;

        var productIds = customer.Cart.Items.Select(i => i.ProductId).ToList();
        var productsResult = await sender.Send(new ProductsQuery(productIds));

        if (productsResult is null || !productsResult.Any())
            return CartErrors.NoProductsFoundForCartItems;

        var checkoutItems = new List<CheckoutItem>();
        foreach (var cartItem in customer.Cart.Items)
        {
            var product = productsResult.FirstOrDefault(p => p.Id == cartItem.ProductId);
            if (product is null)
                return CartErrors.NoProductFoundForCartItem(cartItem.ProductId);

            checkoutItems.Add(CheckoutItem.Create(
                cartItem.ProductId,
                cartItem.Quantity,
                product.BasePrice));
        }

        var createResult = customer.CreateCheckoutSession(checkoutItems);
        if (createResult.IsError)
            return createResult.Errors;

        await usersRepository.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }

    public async Task<ErrorOr<Success>> CancelCheckoutSession(CancellationToken cancellationToken = default)
    {
        var customer = await usersRepository.GetCustomerAsync(customerId);

        if (customer is null)
            return CustomerErrors.CustomerNotFound;

        var cancelResult = customer.CancelCheckoutSession();

        if (cancelResult.IsError)
            return cancelResult.Errors;

        await usersRepository.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }

    public async Task<ErrorOr<(Guid PaymentId, string PaymentUrl)>> Checkout(Guid addressId, CancellationToken cancellationToken = default)
    {
        //@TODO: is this the best way to validate the checkout process?
        // here we fetch customer cart, invoice from invoices module, and products from products module
        var customer = await usersRepository.GetCustomerDetailsAsync(customerId, cancellationToken);

        if (customer is null)
            return Error.NotFound("Customer.NotFound", "The customer was not found.");

        if (customer.Cart is null || customer.Cart.Items.Any() == false)
            return Error.Failure("Cart.Empty", "The cart is empty.");

        if (customer.ActiveCheckoutSession is null)
            return Error.Failure("CheckoutSession.NotFound", "There is no active checkout session for the customer.");

        var setAddressResult = customer.SetAddressForOrder(addressId);

        if (setAddressResult.IsError)
            return setAddressResult.Errors;

        var productsIds = customer.Cart.Items.Select(i => i.ProductId).ToList();
        var products = await sender.Send(new ProductsQuery(productsIds), cancellationToken);

        foreach (var item in customer.ActiveCheckoutSession.Items)
        {
            var product = products?.FirstOrDefault(p => p.Id == item.ProductId);

            if (product is null)
                return CartErrors.NoProductFoundForCartItem(item.ProductId);

            if (product.BasePrice != item.BasePrice)
                return CheckoutSessionErrors.PriceMismatch;
        }

        var paymentSession = await sender.Send(
            new CreatePaymentSessionRequest(
                customerId,
                customer.ActiveCheckoutSession.Id,
                customer.ActiveCheckoutSession.TotalPrice),
            cancellationToken);

        if (paymentSession.IsError)
            return paymentSession.Errors;

        await usersRepository.SaveChangesAsync(cancellationToken);

        return (paymentSession.Value.PaymentId, paymentSession.Value.PaymentUrl);
    }
}