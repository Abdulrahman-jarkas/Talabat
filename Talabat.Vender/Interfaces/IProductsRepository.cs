using Talabat.Vender.Domain.ProductAggregate;
using Talabat.Vender.Domain.ProductAggregate.Entities;

namespace Talabat.Vender.Interfaces;

public interface IProductsRepository
{
	Task Add(Product product);
	Task<Product?> GetById(Guid id);
	Task AddModifierGroup(ModifierGroup modifierGroup);
	Task AddModifier(Modifier modifier);
	Task<List<ModifierGroup>> GetModifierGroups(List<Guid> ids);
	Task SaveChanges();
}