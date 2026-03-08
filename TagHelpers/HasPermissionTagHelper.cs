using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Poseidon.TagHelpers
{
    [HtmlTargetElement(Attributes = "has-permission")]
    public class HasPermissionTagHelper : TagHelper
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HasPermissionTagHelper(
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HtmlAttributeName("has-permission")]
        public string Permission { get; set; } = string.Empty;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null)
            {
                output.SuppressOutput();
                return;
            }

            var authorized = await _authorizationService.AuthorizeAsync(user, Permission);

            if (!authorized.Succeeded)
            {
                output.SuppressOutput();
            }

            output.Attributes.RemoveAll("has-permission");
        }
    }
}
