using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MCBAapp.Filters;

public class AuthorizeAdminAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Bonus Material: Implement global authorisation check.
        // Skip authorisation check if the [AllowAnonymous] attribute is present.
        // Another technique to perform the check: x.GetType() == typeof(AllowAnonymousAttribute)
        //if(context.ActionDescriptor.EndpointMetadata.Any(x => x is AllowAnonymousAttribute))
        //    return;

        var userName = context.HttpContext.Session.GetString("AdminUsername");
        if(string.IsNullOrEmpty(userName))
            context.Result = new RedirectToActionResult("Index", "Home", null);
    }    
}