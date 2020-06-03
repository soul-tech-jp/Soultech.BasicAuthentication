using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;

namespace Soultech.BasicAuthentication.Test.Mocks
{
    public class MockClaimsPrincipal : ClaimsPrincipal
    {
        public MockClaimsPrincipal(IdentityUser identityUser) : base(
            new GenericIdentity(identityUser.UserName, "Basic"))
        {
        }
    }
}