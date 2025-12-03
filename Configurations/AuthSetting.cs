using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Poseidon.Configurations
{
    public class AuthSetting
    {
        public string CookieName { get; set; }
        public int CookieExpireMinutes { get; set; } = 60;
        public bool UseSlidingExpiration { get; set; } = true;
        public bool EnableAbsoluteExpiration { get; set; } = false;
        public int AbsoluteExpireHours { get; set; } = 8;
    }
    public class InactivitySetting
    {
        public int MaxInactiveTimeSeconds { get; set; }
        public int ForcedLogoutTimeSeconds { get; set; }
    }
    public class CookieClaims
    {
        public ClaimsPrincipal Principal { get; set; }
        public AuthenticationProperties AuthProperties { get; set; }
    }

}
