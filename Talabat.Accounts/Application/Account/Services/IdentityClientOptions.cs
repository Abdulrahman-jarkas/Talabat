namespace Talabat.Accounts.Application.Account.Services;

public class IdentityClientOptions
{
    public const string SectionName = "IdentityClient";

    public string TokenEndpoint { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string Scope { get; set; } = null!;
    public string BaseAddress { get; set; } = null!;
}
