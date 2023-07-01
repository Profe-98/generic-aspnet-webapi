using Microsoft.AspNetCore.SignalR;

namespace JellyFishBackend.SignalR.Hub
{
    public static class HubCallerContextExtension
    {

        public static Guid ExtractGuidFromClaims(this HubCallerContext hubCallerContext)
        {
            var claimsPrincipal = hubCallerContext.User;
            if (claimsPrincipal == null)
                return Guid.Empty;

            if (claimsPrincipal.Claims == null || claimsPrincipal.Claims.Count() == 0)
            {
                return Guid.Empty;
            }
            var claim = claimsPrincipal.Claims.ToList().Find(x => x.Type == "uuid");
            if (claim == null)
                return Guid.Empty;

            return Guid.Parse(claim.Value);
        }
    }
}
