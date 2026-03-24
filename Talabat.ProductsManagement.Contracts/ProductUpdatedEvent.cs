using MediatR;

namespace Talabat.ProductsManagement.Contracts;

public record ProductUpdatedEvent(Guid ProductId) : INotification;
