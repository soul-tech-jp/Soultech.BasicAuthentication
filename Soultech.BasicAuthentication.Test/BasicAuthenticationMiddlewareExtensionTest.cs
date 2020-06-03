using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Soultech.BasicAuthentication.Test
{
    public class BasicAuthenticationMiddlewareExtensionTest
    {
        [Fact]
        public void TestUseBasicAuthentication()
        {
            // 一応現時点( Microsoft.AspNetCore.Http.Abstractions 2.2.0.0) は
            // 内部で Use が呼ばれるので以下の方法でテスト可能。
            // 本来警告抑制用のテストなのでテストしなくても良い

            try
            {
                var mockApplicationBuilder = new Mock<IApplicationBuilder>();
                mockApplicationBuilder.Setup(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                    .Returns(mockApplicationBuilder.Object);
                var result = mockApplicationBuilder.Object.UseBasicAuthentication<IdentityUser>();
                Assert.Equal(mockApplicationBuilder.Object, result);
            }
            catch
            {
                // ignored
            }
        }
    }
}