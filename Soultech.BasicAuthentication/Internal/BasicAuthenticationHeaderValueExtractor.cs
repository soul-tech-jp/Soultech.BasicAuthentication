using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Soultech.BasicAuthentication.Internal
{
    public static class BasicAuthenticationHeaderValueExtractor
    {
        /// <summary>
        /// HTTPコンテキストからBASIC認証用のヘッダー値を取得する
        /// </summary>
        /// <param name="context">コンテキスト</param>
        /// <returns>BASIC認証用のヘッダー値、ヘッダにBASIC認証用の値が存在しない場合は null</returns>
        public static BasicAuthenticationHeaderValue? ExtractBasicAuthHeaderValue(HttpContext context)
        {
            var headerValue = context.Request.Headers["Authorization"]
                .FirstOrDefault(x => x.StartsWith("Basic"));
            return string.IsNullOrEmpty(headerValue)
                ? null
                : BasicAuthenticationHeaderValue.Decode(headerValue);
        }
    }
}