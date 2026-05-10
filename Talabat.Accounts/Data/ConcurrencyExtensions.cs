using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Talabat.Accounts.Data;

internal static class ConcurrencyExtensions
{
    internal static ErrorOr<Success> EnsureVersion(byte[] currentVersion, string expectedVersion, Func<Error> errorFactory)
    {
        if (Convert.ToBase64String(currentVersion) != expectedVersion)
            return errorFactory();

        return Result.Success;
    }

    internal static async Task<ErrorOr<T>> SaveWithConcurrencyAsync<T>(
        this DbContext dbContext,
        Func<T> successFactory,
        Func<Error> concurrencyError,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return successFactory();
        }
        catch (DbUpdateConcurrencyException)
        {
            return concurrencyError();
        }
    }
}
