using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Talabat.Accounts.Domain.AccountAggregate;
using Talabat.Accounts.Domain.RoleAggregate;
using Talabat.SharedKernal;

namespace Talabat.Accounts.Data;

public class AccountsDbContext : DbContext
{
    private readonly IPublisher _publisher;

    internal DbSet<Account> Accounts { get; set; }
    internal DbSet<Role> Roles { get; set; }

    public AccountsDbContext(DbContextOptions<AccountsDbContext> options, IPublisher publisher) : base(options)
    {
        _publisher = publisher;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Accounts");

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker.Entries<AggregateRoot>()
            .SelectMany(e => e.Entity.PopDomainEvents())
            .ToList();

        foreach (var domainEvent in domainEvents)
            await _publisher.Publish(domainEvent, cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }
}
