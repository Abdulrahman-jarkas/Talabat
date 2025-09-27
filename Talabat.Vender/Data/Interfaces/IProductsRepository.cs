using Talabat.Vender.Domain.ProductAggregate;
using Talabat.Vender.Domain.ProductAggregate.Entities;

namespace Talabat.Vender.Infrastructure.Persistence.Repositories;

public interface IProductsRepository
{
	Task Add(Product product);
	Task AddModifierGroup(ModifierGroup modifierGroup);
	Task AddModifier(Modifier modifier);
	Task SaveChanges();
}