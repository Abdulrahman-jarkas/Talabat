using MediatR;

namespace Talabat.ProductsManagement.Contracts;

public record ProductDetailsQuery(int ProductId) : IRequest<ProductResponse?>;

public record ProductResponse(int Id, string Title, decimal Price, List<ModifierGroupResponse> ModifierGroups);

public record ModifierResponse(int Id, string Title, decimal Price, List<ModifierSubGroupResponse> ModifierSubGroups);

public record ModifierGroupResponse(Guid Id, string Title, int Min, int Max, List<ModifierResponse> Modifiers);

public record ModifierSubGroupResponse(Guid Id, string Title, int Min, int Max, List<ModifierForSubGroupResponse> Modifiers);

public record ModifierForSubGroupResponse(int Id, string Title, decimal Price);



