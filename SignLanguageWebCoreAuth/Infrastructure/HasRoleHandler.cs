using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SignLanguageWebCoreAuth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SignLanguageWebCoreAuth.Infrastructure
{
    public class HasRoleHandler : AuthorizationHandler<HasRoleRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   HasRoleRequirement requirement)
        {
            // Save User object to access claims
            var redirectContext = context.Resource as AuthorizationFilterContext;

            var user = context.User; if (!user.HasClaim(c => c.Type == ClaimTypes.Role))
            {
                redirectContext.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var since = user.HasClaim(ClaimTypes.Role, requirement.Role) || user.HasClaim(ClaimTypes.Role, "Admin");
            if (since)
                context.Succeed(requirement);
            else
            {
                redirectContext.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

       
    }
}
