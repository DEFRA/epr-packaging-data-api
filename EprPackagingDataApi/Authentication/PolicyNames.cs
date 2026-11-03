using System.Diagnostics.CodeAnalysis;

namespace EprPackagingDataApi.Authentication;

[ExcludeFromCodeCoverage]
public static class PolicyNames
{
    public const string Admin = nameof(Scopes.Admin);
    public const string Read = nameof(Scopes.Read);
    public const string Write = nameof(Scopes.Write);
}
