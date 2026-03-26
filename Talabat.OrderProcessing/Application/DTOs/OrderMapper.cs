using Talabat.OrderProcessing.Domain.OrderAggregate;

namespace Talabat.OrderProcessing.Application.DTOs;

public static class OrderMapper
{
	public static OrderDetailsDto ToDto(this Order order)
	{
		return new OrderDetailsDto
		{
			Id = order.Id,
			OrderStatus = order.Status.CurrentStatus.ToString(),
			Payment = new OrderPaymentDto
			{
				PaymentStatus = order.PaymentStatus.ToString(),
				//PaymentMethod = order.PaymentMethod.ToString()
			},
			Items = order.Items.Select(i => new OrderItemDto
			{
				ProductId = i.ProductId,
				ProductPrice = i.ProductPrice,
				Quantity = i.Quantity,
				Note = i.Note,
				TotalPrice = i.GetTotalPrice(),
				//Modifiers = i.Modifiers.Select(g => g.ToDto()).ToList()
			}).ToList(),
			//ServiceFees = order.ServiceFees,
			Subtotal = order.Subtotal,
			Total = order.Total,
		};
	}

	//public static OrderItemDto.ModifierDto ToDto(this Modifier modifier)
	//{
	//	return new OrderItemDto.ModifierDto()
	//	{
	//		Id = modifier.Id,
	//		Name = modifier.Name,
	//		Price = modifier.Price,
	//		GroupId = modifier.GroupId,
	//		GroupName = modifier.GroupTitle
	//	};
	//}
}