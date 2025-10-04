using ErrorOr;
using Talabat.Vender.Core.Common;

namespace Talabat.Vender.Domain.ProductAggregate.Entities;

public class Modifier : Entity
{
	public string Title { get;  set; } = string.Empty;
	public decimal Price { get;  set; }

	public Modifier(
		string title,
		decimal price)
	{
		Title = title;
		Price = price;
	}

	public Modifier()
	{
	}
}
