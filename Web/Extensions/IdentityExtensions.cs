using System.Security.Claims;
using System.Security.Principal;

namespace Web.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetRole(this IIdentity identity)
        {
            Claim claim = ((ClaimsIdentity)identity).FindFirst(ClaimTypes.Role);
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetFullName(this IIdentity identity)
        {
            Claim claim = ((ClaimsIdentity)identity).FindFirst("FullName");
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetCareHomeId(this IIdentity identity)
        {
            Claim claim = ((ClaimsIdentity)identity).FindFirst("CareHomeId");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string GetCareHomeName(this IIdentity identity)
        {
            Claim claim = ((ClaimsIdentity)identity).FindFirst("CareHomeName");
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}
