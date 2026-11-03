using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace EprPackagingDataApi.Test.Health;

/// <summary>
/// CDP polls the health check to decide whether the container is alive. It must never require
/// authentication, whatever the caller sends. If any of these fail, CDP will treat the service as
/// unhealthy and stop routing to it.
/// </summary>
public class HealthEndpointsAnonymousTest
{
    public static readonly TheoryData<string> HealthRoutes = new() { "/health" };

    public static TheoryData<string, string?> HealthRoutesWithAuthorizationHeaders()
    {
        string?[] headers =
        [
            null,
            "Basic !!!notbase64",
            "Basic",
            "not a valid header",
            AclTestFactory.Basic(AclTestFactory.ClientId, "wrong-secret").ToString(),
            AclTestFactory.Basic("UnknownClient", AclTestFactory.Secret).ToString(),
            "Bearer junk",
            AclTestFactory.Basic(AclTestFactory.ClientId, AclTestFactory.Secret).ToString(),
        ];

        var data = new TheoryData<string, string?>();
        foreach (var route in HealthRoutes)
        foreach (var header in headers)
            data.Add(route, header);
        return data;
    }

    [Theory]
    [MemberData(nameof(HealthRoutesWithAuthorizationHeaders))]
    public async Task Health_check_is_ok_whatever_authorization_is_sent(string route, string? authorization)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var factory = new AclTestFactory();
        using var client = factory.CreateClient();
        if (authorization is not null)
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorization);

        var response = await client.GetAsync(route, cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Every_health_endpoint_is_explicitly_anonymous_and_has_no_authorization_requirement()
    {
        await using var factory = new AclTestFactory();
        var endpoints = factory.Services.GetRequiredService<EndpointDataSource>().Endpoints
            .OfType<RouteEndpoint>()
            .Where(e => e.RoutePattern.RawText?.TrimStart('/').StartsWith("health", StringComparison.OrdinalIgnoreCase) == true)
            .ToList();

        Assert.Equal(
            HealthRoutes.Select(r => r.Data).Order(),
            endpoints.Select(e => "/" + e.RoutePattern.RawText!.TrimStart('/')).Order());

        Assert.All(endpoints, endpoint =>
        {
            Assert.NotNull(endpoint.Metadata.GetMetadata<IAllowAnonymous>());
            Assert.Empty(endpoint.Metadata.GetOrderedMetadata<IAuthorizeData>());
            Assert.Empty(endpoint.Metadata.GetOrderedMetadata<AuthorizationPolicy>());
        });
    }

    [Fact]
    public async Task There_is_no_fallback_authorization_policy()
    {
        // A fallback policy would apply to every endpoint without its own, including any health
        // check someone forgets to mark anonymous. Endpoints opt in to authentication instead.
        await using var factory = new AclTestFactory();
        var policyProvider = factory.Services.GetRequiredService<IAuthorizationPolicyProvider>();

        Assert.Null(await policyProvider.GetFallbackPolicyAsync());
    }
}
