using Xunit;

namespace Soultech.BasicAuthentication.Test
{
    public class BasicAuthenticationHeaderValueTest
    {
        [Theory]
        // ReSharper disable once StringLiteralTypo
        [InlineData("Basic dXNlcjpwYXNz", "user", "pass")]
        [InlineData("Basic dXNlckBzb3VsLXRlY2guanA6UGFzczEyMzQ=", "user@soul-tech.jp", "Pass1234")]
        [InlineData("Basic OlBhc3MxMjM0", "", "Pass1234")]
        [InlineData("Basic dXNlckBzb3VsLXRlY2guanA6", "user@soul-tech.jp", "")]
        [InlineData("Basic dXNlcjpwYXNzOndvcmQ=", "user", "pass:word")] // コロンを含むパスワードは利用可能
        public void TestDecodeNormalValue(string rawValue, string user, string password)
        {
            var headerValue = BasicAuthenticationHeaderValue.Decode(rawValue);

            Assert.NotNull(headerValue);
            Assert.Equal(headerValue?.User, user);
            Assert.Equal(headerValue?.Password, password);
        }
    }
}