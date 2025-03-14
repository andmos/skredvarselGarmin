using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using SkredvarselGarminWeb.Services;

namespace SkredvarselGarminWeb.Configuration;

public partial class GarminAuthenticationHandler : AuthenticationHandler<GarminAuthenticationSchemeOptions>
{
    public GarminAuthenticationHandler(
        IOptionsMonitor<GarminAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey(HeaderNames.Authorization))
        {
            return Task.FromResult(AuthenticateResult.Fail("Header Not Found."));
        }

        var header = Request.Headers[HeaderNames.Authorization].ToString();
        var tokenMatch = GarminAuthenticationHeader().Match(header);

        if (tokenMatch.Success)
        {
            var watchId = tokenMatch.Groups["token"].Value;

            var garminAuthenticationService = Request.HttpContext.RequestServices.GetRequiredService<IGarminAuthenticationService>();
            var activeAgreement = garminAuthenticationService.DoesWatchHaveActiveAgreement(watchId);
            if (activeAgreement)
            {
                var claims = Array.Empty<Claim>();
                var claimsIdentity = new ClaimsIdentity(claims, nameof(GarminAuthenticationHandler));
                var ticket = new AuthenticationTicket(
                        new ClaimsPrincipal(claimsIdentity), Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
        }

        return Task.FromResult(AuthenticateResult.Fail("Did not find active agreement for watch."));
    }

    [GeneratedRegex("Garmin (?<token>.*)")]
    private static partial Regex GarminAuthenticationHeader();
}
