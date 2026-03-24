using MediatR;

namespace Talabat.ProductsManagement.Contracts;

public record ProductDeletedEvent(Guid ProductId) : INotification;
