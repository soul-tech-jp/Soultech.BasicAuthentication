using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Soultech.BasicAuthentication.Test.Mocks;
using Xunit;

namespace Soultech.BasicAuthentication.Test
{
    public class BasicAuthenticationMiddlewareTest
    {
        private Mock<ILogger<BasicAuthenticationMiddleware<IdentityUser>>> MockLogger { get; } =
            new Mock<ILogger<BasicAuthenticationMiddleware<IdentityUser>>>();

        private Mock<MockSignInManager<IdentityUser>> MockSignInManager { get; } =
            new Mock<MockSignInManager<IdentityUser>>();

        private Mock<MockUserManager<IdentityUser>> MockUserManager { get; } =
            new Mock<MockUserManager<IdentityUser>>();


        /// <summary>
        /// 空のコンテキストの場合何もしない 
        /// </summary>
        [Fact]
        public async void TestEmptyContext()
        {
            var context = new DefaultHttpContext();
            var next = new RequestDelegate(_ => Task.CompletedTask);

            var oldUser = context.User;
            var middleware = new BasicAuthenticationMiddleware<IdentityUser>(next);
            await middleware.InvokeAsync(context);

            // User が変更されないこと
            Assert.Equal(oldUser, context.User);
            // 認証されていないこと
            Assert.False(context.User.Identity.IsAuthenticated);
        }

        [Theory]
        [InlineData("Basic dXNlcjpwYXNzOndvcmQ=", "user", "pass:word")]
        [InlineData("Basic dXNlcjpwYXNzd29yZA==", "user", "password")]
        public async Task TestValidAuth(string header, string user, string password)
        {
            var currentUser = new IdentityUser
            {
                UserName = user
            };

            SetupMockUserManager(user, true, false, false, currentUser);
            SetupMockSignInManagerSuccess(password, currentUser);

            var sp = new ServiceCollection()
                .Configure<BasicAuthenticationOptions>(options => { options.FindsByName = true; })
                .AddTransient<UserManager<IdentityUser>>(_ => MockUserManager.Object)
                .AddTransient<SignInManager<IdentityUser>>(_ => MockSignInManager.Object)
                .AddTransient(_ => MockLogger.Object)
                .BuildServiceProvider();

            var contextMock = BuildContextMock(header, sp);

            var next = new RequestDelegate(_ => Task.CompletedTask);

            var middleware = new BasicAuthenticationMiddleware<IdentityUser>(next);
            await middleware.InvokeAsync(contextMock.Object);
            Assert.True(contextMock.Object.User.Identity.IsAuthenticated);
        }

        [Theory]
        [InlineData("Basic dXNlcjpwYXNzd29yZA==", "user", "password", true, false, false)]
        [InlineData("Basic dXNlcjpwYXNzd29yZA==", "user", "password", false, true, false)]
        [InlineData("Basic dXNlcjpwYXNzd29yZA==", "user", "password", false, false, true)]
        [InlineData("Basic dXNlcjpwYXNzd29yZA==", "user", "password", true, true, true)]
        public async Task TestAuthOptions(string header, string user, string password, bool asName, bool asId,
            bool asEmail)
        {
            var currentUser = new IdentityUser
            {
                UserName = user
            };

            SetupMockUserManager(user, asName, asId, asEmail, currentUser);

            SetupMockSignInManagerSuccess(password, currentUser);

            var sp = new ServiceCollection()
                .Configure<BasicAuthenticationOptions>(options =>
                {
                    options.FindsByName = asName;
                    options.FindsById = asId;
                    options.FindsByEmail = asEmail;
                })
                .AddTransient<UserManager<IdentityUser>>(_ => MockUserManager.Object)
                .AddTransient<SignInManager<IdentityUser>>(_ => MockSignInManager.Object)
                .AddTransient(_ => MockLogger.Object)
                .BuildServiceProvider();

            var contextMock = BuildContextMock(header, sp);

            var next = new RequestDelegate(_ => Task.CompletedTask);

            var middleware = new BasicAuthenticationMiddleware<IdentityUser>(next);
            await middleware.InvokeAsync(contextMock.Object);
            Assert.True(contextMock.Object.User.Identity.IsAuthenticated);
        }

        [Theory]
        [InlineData("Basic dXNlcjpwYXNzd29yZA==", "user", true, true, true)]
        public async Task TestUserNotFound(string header, string user, bool asName, bool asId,
            bool asEmail)
        {
            SetupMockUserManager(user, asName, asId, asEmail, null);

            var sp = new ServiceCollection()
                .Configure<BasicAuthenticationOptions>(options =>
                {
                    options.FindsByName = asName;
                    options.FindsById = asId;
                    options.FindsByEmail = asEmail;
                })
                .AddTransient<UserManager<IdentityUser>>(_ => MockUserManager.Object)
                .AddTransient<SignInManager<IdentityUser>>(_ => MockSignInManager.Object)
                .AddTransient(_ => MockLogger.Object)
                .BuildServiceProvider();

            var contextMock = BuildContextMock(header, sp);

            var next = new RequestDelegate(_ => Task.CompletedTask);

            var middleware = new BasicAuthenticationMiddleware<IdentityUser>(next);
            await middleware.InvokeAsync(contextMock.Object);
            Assert.False(contextMock.Object.User.Identity.IsAuthenticated);
        }

        [Theory]
        [InlineData("Basic dXNlcjpwYXNzd29yZA==", "user", true, true, true)]
        public async Task TestSignInFailed(string header, string user, bool asName, bool asId,
            bool asEmail)
        {
            var currentUser = new IdentityUser
            {
                UserName = user
            };

            SetupMockUserManager(user, asName, asId, asEmail, currentUser);

            SetupMockSignInManagerFail();

            var sp = new ServiceCollection()
                .Configure<BasicAuthenticationOptions>(options =>
                {
                    options.FindsByName = asName;
                    options.FindsById = asId;
                    options.FindsByEmail = asEmail;
                })
                .AddTransient<UserManager<IdentityUser>>(_ => MockUserManager.Object)
                .AddTransient<SignInManager<IdentityUser>>(_ => MockSignInManager.Object)
                .AddTransient(_ => MockLogger.Object)
                .BuildServiceProvider();

            var contextMock = BuildContextMock(header, sp);

            var next = new RequestDelegate(_ => Task.CompletedTask);

            var middleware = new BasicAuthenticationMiddleware<IdentityUser>(next);
            await middleware.InvokeAsync(contextMock.Object);
            Assert.False(contextMock.Object.User.Identity.IsAuthenticated);
        }

        private void SetupMockUserManager(string userKey, bool asName, bool asId, bool asEmail, IdentityUser? user)
        {
            MockUserManager.Setup(x => x.FindByNameAsync(userKey))
                .Returns(Task.Run(() => (asName ? user : null)!));
            MockUserManager.Setup(x => x.FindByIdAsync(userKey))
                .Returns(Task.Run(() => (asId ? user : null)!));
            MockUserManager.Setup(x => x.FindByEmailAsync(userKey))
                .Returns(Task.Run(() => (asEmail ? user : null)!));
        }

        private void SetupMockSignInManagerSuccess(string password, IdentityUser user)
        {
            MockSignInManager.Setup(x => x.CheckPasswordSignInAsync(
                    user,
                    password, It.IsAny<bool>()))
                .Returns(Task.Run(() => SignInResult.Success));

            MockSignInManager.Setup(x => x.CreateUserPrincipalAsync(user))
                .Returns<IdentityUser>(x => Task.Run(() => (ClaimsPrincipal) new MockClaimsPrincipal(x)));
        }

        private void SetupMockSignInManagerFail()
        {
            MockSignInManager.Setup(x => x.CheckPasswordSignInAsync(It.IsAny<IdentityUser>(),
                    It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(Task.Run(() => SignInResult.Failed));
        }

        private static Mock<HttpContext> BuildContextMock(string header, IServiceProvider sp)
        {
            var contextMock = new Mock<HttpContext>();
            contextMock.SetupGet(x => x.Request)
                .Returns(() =>
                {
                    var request = new DefaultHttpRequest(new DefaultHttpContext());
                    request.Headers.Add("Authorization", header);
                    return request;
                });

            var identityMock = new Mock<IIdentity>();
            identityMock.SetupGet(x => x.IsAuthenticated)
                .Returns(false);
            contextMock.SetupProperty(x => x.User, new ClaimsPrincipal(identityMock.Object));

            contextMock.SetupGet(x => x.RequestServices)
                .Returns(sp);

            return contextMock;
        }
    }
}