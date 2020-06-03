using Microsoft.AspNetCore.Identity;
using Moq;

namespace Soultech.BasicAuthentication.Test.Mocks
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MockUserManager<TUser> : UserManager<TUser> where TUser : class
    {
        public MockUserManager() : base(new Mock<IUserStore<TUser>>().Object, 
            null,
            null,
            null,
            null,
            null,
            null,
            null, null)
        {
        }
    }
}