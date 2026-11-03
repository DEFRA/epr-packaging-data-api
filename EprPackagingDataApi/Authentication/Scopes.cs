using System.Diagnostics.CodeAnalysis;

namespace EprPackagingDataApi.Authentication;

[ExcludeFromCodeCoverage]
public static class Scopes
{
    public const string Admin = "admin";
    public const string Read = "read";
    public const string Write = "write";

    public static readonly IReadOnlyList<string> All = [Admin, Read, Write];
}
