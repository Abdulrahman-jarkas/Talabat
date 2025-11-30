using ErrorOr;
using MediatR;
using Talabat.OrderProcessing.Application.DTOs;
using Talabat.OrderProcessing.Data.Repositories;
using Talabat.OrderProcessing.Domain.CheckoutSessionAggregate;
using Talabat.OrderProcessing.Domain.OrderAggregate;
using Talabat.OrderProcessing.Endpoints.CreateOrder;
using Talabat.Payments.Contracts;
using Talabat.ProductsManagement.Contracts;
using Talabat.Taxes.Contracts;


namespace Talabat.OrderProcessing.Application.Services;

public class OrderService(ISender sender, IOrderRepository orderRepository, ICheckoutSessionsRepository checkoutSessionsRepository) : IOrderService
{
	//@TODO: we need a cart and cart items instead of repeate the order items

	public async Task<ErrorOr<OrderDetailsDto>> CreateOrderAsync(
		CreateOrderRequest request,
		CancellationToken cancellationToken = default)
	{
		// get country id from the token of the user
		var orderItems = new List<OrderItem>();

		var serviceFeesData = await sender.Send(new GetServiceFeesQuery(1));

		if (serviceFeesData == null) 
			return Error.NotFound($"Service Fees with couuntry id {1} not found");

		var serviceFees = serviceFeesData.Value + (serviceFeesData.VatPercentage / 100);

		foreach (var itemRequest in request.Items)
		{
			var product = await sender.Send(new ProductDetailsQuery(itemRequest.ProductId), cancellationToken);

			if (product is null)
				return Error.NotFound($"Product with ID {itemRequest.ProductId} not found.");

			var itemResult = await CreateOrderItemAsync(itemRequest, product);
			if (itemResult.IsError)
				return itemResult.Errors;
			else
				orderItems.Add(itemResult.Value);
		}

		var order = new Order(orderItems, PaymentMethodValues.Cash, serviceFees);

		await orderRepository.CreateAsync(order, cancellationToken);
		await orderRepository.SaveChangesAsync(cancellationToken);

		return order.ToDto();
	}

	private async Task<ErrorOr<OrderItem>> CreateOrderItemAsync(CreateOrderItemRequest request, ProductResponse product)
	{
		var errors = new List<Error>();
		var modifiers = new List<Modifier>();

		if(product is null)
			return Error.NotFound("Product not found.");

		// get required group from product response (required group are the groups that i's have min : 1)
		var requiredGroupsIds = product?.ModifierGroups
			.Where(g => g.Min > 0)
			.Select(g => g.Id)
			.ToList() ?? new List<Guid>();

		// Check if all required groups are present in the request
		foreach (var group in requiredGroupsIds)
		{
			if (!request.Groups.Any(g => g.Id == group))
			{
				errors.Add(Error.Validation($"You must select at least one modifier from the required group with ID {group} for product '{product.Title}'."));
			}
		}

		foreach (var reqGroup in request.Groups)
		{
			var groupInfo = product!.ModifierGroups.FirstOrDefault(mg => mg.Id == reqGroup.Id);

			if (groupInfo is null)
			{
				errors.Add(Error.Validation($"Modifier group with ID {reqGroup.Id} does not exist for product '{product.Title}'."));
				continue;
			}

			var selectedCount = reqGroup.Modifiers.Count;

			// Bounds check
			if (selectedCount < groupInfo.Min || selectedCount > groupInfo.Max)
			{
				errors.Add(Error.Validation(
					$"You must select between {groupInfo.Min} and {groupInfo.Max} modifiers in group '{groupInfo.Title}' for product '{product.Title}'. You selected {selectedCount}."));
			}

			foreach (var reqModifier in reqGroup.Modifiers)
			{
				var productModifier = groupInfo.Modifiers.FirstOrDefault(m => m.Id == reqModifier.Id);
				if (productModifier is null)
				{
					errors.Add(Error.Validation($"Modifier with ID {reqModifier.Id} does not exist in group '{groupInfo.Title}' for product '{product.Title}'."));
					continue;
				}

				modifiers.Add(new Modifier
				{
					Id = productModifier.Id,
					Name = productModifier.Title,
					Price = productModifier.Price,
					GroupId = groupInfo.Id,
					GroupTitle = groupInfo.Title
				});

				// get required sub groups from product modifier
				var requiredSubGroupsIds = productModifier.ModifierSubGroups
					.Where(sg => sg.Min > 0)
					.Select(sg => sg.Id)
					.ToList();

				// Check if all required sub groups are present in the request
				foreach (var subGroupId in requiredSubGroupsIds)
				{
					if (!reqModifier.SubGroups.Any(sg => sg.Id == subGroupId))
					{
						errors.Add(Error.Validation($"You must select at least one modifier from the required subgroup with ID {subGroupId} for product '{product.Title}'."));
					}
				}

				// Validate sub groups
				foreach (var reqSubGroup in reqModifier.SubGroups)
				{
					var subGroupInfo = productModifier.ModifierSubGroups.FirstOrDefault(sg => sg.Id == reqSubGroup.Id);
					if (subGroupInfo is null)
					{
						errors.Add(Error.Validation($"Modifier subgroup with ID {reqSubGroup.Id} does not exist in modifier '{productModifier.Title}' for product '{product.Title}'."));
						continue;
					}

					var subSelectedCount = reqSubGroup.Modifiers.Count;
					// Bounds check
					if (subSelectedCount < subGroupInfo.Min || subSelectedCount > subGroupInfo.Max)
					{
						errors.Add(Error.Validation(
							$"You must select between {subGroupInfo.Min} and {subGroupInfo.Max} modifiers in subgroup '{subGroupInfo.Title}' for product '{product.Title}'. You selected {subSelectedCount}."));
					}

					foreach (var reqSubModifier in reqSubGroup.Modifiers)
					{
						var productSubModifier = subGroupInfo.Modifiers.FirstOrDefault(m => m.Id == reqSubModifier);
						if (productSubModifier is null)
						{
							errors.Add(Error.Validation($"Modifier with ID {reqSubModifier} does not exist in subgroup '{subGroupInfo.Title}' for product '{product.Title}'."));
							continue;
						}

						modifiers.Add(new Modifier
						{
							Id = productSubModifier.Id,
							Name = productSubModifier.Title,
							Price = productSubModifier.Price,
							GroupId = subGroupInfo.Id,
							GroupTitle = subGroupInfo.Title
						});
					}
				}
			}
		}

		if (errors.Count > 0)
			return errors;

		// get the country code from the token of the user or get it vendor of the product 
		var tax = await sender.Send(new GetTaxQuery(product.TaxId));

		if (tax is null)
		{
			return Error.NotFound("Tax policy not found for the specified country.");
		}

		var orderItem = new OrderItem
		{
			ProductId = product!.Id,
			ProductPrice = product.Price,
			Quantity = request.Quantity,
			Note = request.Note,
			Modifiers = modifiers,
			Vat = tax.VatPercentage
		};

		return orderItem;
	}

	public async Task<ErrorOr<Success>> Accept(int orderId, CancellationToken cancellationToken = default)
	{
		var order = await orderRepository.GetByIdAsync(orderId, cancellationToken);

		if (order is null)
			return Error.NotFound($"Order with ID {orderId} not found.");

		var result = order.Accept();

		if (result.IsError)
			return result;

		await orderRepository.UpdateAsync(order, cancellationToken);
		await orderRepository.SaveChangesAsync(cancellationToken);

		return Result.Success;
	}

	public async Task<ErrorOr<Success>> Reject(int orderId, CancellationToken cancellationToken = default)
	{
		var order = await orderRepository.GetByIdAsync(orderId, cancellationToken);

		if (order is null)
			return Error.NotFound($"Order with ID {orderId} not found.");

		var result = order.Reject();

		if (result.IsError)
			return result;

		await orderRepository.UpdateAsync(order, cancellationToken);
		await orderRepository.SaveChangesAsync(cancellationToken);
		return Result.Success;
	}

	public async Task<ErrorOr<Success>> Ship(int orderId, CancellationToken cancellationToken = default)
	{
		var order = await orderRepository.GetByIdAsync(orderId, cancellationToken);

		if (order is null)
			return Error.NotFound($"Order with ID {orderId} not found.");

		var result = order.Ship();

		if (result.IsError)
			return result;

		await orderRepository.UpdateAsync(order, cancellationToken);
		await orderRepository.SaveChangesAsync(cancellationToken);

		return Result.Success;
	}

	public async Task<ErrorOr<Success>> Cancel(int orderId, CancellationToken cancellationToken = default)
	{
		var order = await orderRepository.GetByIdAsync(orderId, cancellationToken);

		if (order is null)
			return Error.NotFound($"Order with ID {orderId} not found.");

		var result = order.Cancel();

		if (result.IsError)
			return result;

		await orderRepository.UpdateAsync(order, cancellationToken);
		await orderRepository.SaveChangesAsync(cancellationToken);

		return Result.Success;
	}

	public async Task<ErrorOr<Success>> Deliver(int orderId, CancellationToken cancellationToken = default)
	{
		var order = await orderRepository.GetByIdAsync(orderId, cancellationToken);

		if (order is null)
			return Error.NotFound($"Order with ID {orderId} not found.");

		var result = order.Deliver();

		if (result.IsError)
			return result;

		await orderRepository.UpdateAsync(order, cancellationToken);
		await orderRepository.SaveChangesAsync(cancellationToken);

		return Result.Success;
	}

	public async Task<ErrorOr<Success>> CashPay(int orderId, decimal amount, CancellationToken cancellationToken = default)
	{
		var order = await orderRepository.GetByIdAsync(orderId, cancellationToken);

		if (order is null)
			return Error.NotFound($"Order with ID {orderId} not found.");

		var result = order.Pay();

		if (result.IsError)
			return result;

		await orderRepository.UpdateAsync(order, cancellationToken);
		await orderRepository.SaveChangesAsync(cancellationToken);

		return Result.Success;
	}

	public Task<ProductResponse?> GetProductDetailsAsync(int productId, CancellationToken cancellationToken = default)
	{
		return sender.Send(new ProductDetailsQuery(productId), cancellationToken);
	}

	//@TODO: we need a cart and cart items instead of repeate the order items
	public async Task<ErrorOr<string>> StartCheckoutSession(CreateOrderRequest request, CancellationToken cancellationToken = default)
	{
		var orderItems = new List<OrderItem>();

		var serviceFeesData = await sender.Send(new GetServiceFeesQuery(1));

		if (serviceFeesData == null)
			return Error.NotFound($"Service Fees with couuntry id {1} not found");

		var serviceFees = serviceFeesData.Value + (serviceFeesData.VatPercentage / 100);

		foreach (var itemRequest in request.Items)
		{
			var product = await sender.Send(new ProductDetailsQuery(itemRequest.ProductId), cancellationToken);

			if (product is null)
				return Error.NotFound($"Product with ID {itemRequest.ProductId} not found.");

			var itemResult = await CreateOrderItemAsync(itemRequest, product);
			if (itemResult.IsError)
				return itemResult.Errors;
			else
				orderItems.Add(itemResult.Value);
		}


		var checkoutSession = new CheckoutSession(orderItems, serviceFees);

		await checkoutSessionsRepository.CreateAsync(checkoutSession, cancellationToken);
		await checkoutSessionsRepository.SaveChangesAsync(cancellationToken);

		var paymentSession = await sender.Send(new CreatePaymentSessionRequest(checkoutSession.Id, checkoutSession.Total), cancellationToken);

		if(paymentSession.IsError)
			return paymentSession.Errors;

		checkoutSession.SetPaymentSession(paymentSession.Value.PaymentId);

		await checkoutSessionsRepository.UpdateAsync(checkoutSession);
		await checkoutSessionsRepository.SaveChangesAsync();

		return paymentSession.Value.PaymentUrl;
	}

	public async Task<ErrorOr<Success>> CreateOrderFromCheckoutSessionAsync(CheckoutSession checkoutSession, CancellationToken cancellationToken = default)
	{
		var order = new Order(checkoutSession.Items, PaymentMethodValues.Card, checkoutSession.ServiceFees, checkoutSession.PaymentId);

		await orderRepository.CreateAsync(order, cancellationToken);
		await orderRepository.SaveChangesAsync(cancellationToken);

		return Result.Success;
	}
}
