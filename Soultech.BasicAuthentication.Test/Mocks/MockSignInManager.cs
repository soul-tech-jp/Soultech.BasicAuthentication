using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Soultech.BasicAuthentication.Test.Mocks
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MockSignInManager<TUser> : SignInManager<TUser> where TUser : class
    {
        public MockSignInManager() 
            : base(new Mock<MockUserManager<TUser>>().Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<TUser>>().Object,
                null, null, null)
        {
        }
    }
}