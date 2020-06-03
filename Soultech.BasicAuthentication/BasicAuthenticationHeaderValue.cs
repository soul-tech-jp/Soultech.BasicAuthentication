using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Soultech.BasicAuthentication
{
    // ReSharper disable once CommentTypo
    /// <summary>
    /// Basic認証の HTTPヘッダの値
    /// </summary>
    /// <example>
    /// BasicAuthenticationHeaderValue headerValue = BasicAuthenticationHeaderValue.Decode("Basic dXNlcjpwYXNz");
    /// Debug.Assert(headerValue.User == "user");
    /// Debug.Assert(headerValue.Password = "pass");
    /// </example>
    public class BasicAuthenticationHeaderValue
    {
        /// <summary>
        /// Basic認証用のAuthorizationヘッダの値
        /// </summary>
        private static readonly Regex BasicHeaderValueRegex = new Regex(@"^\s*Basic\s+?(.*)$", RegexOptions.Compiled);

        /// <summary>
        /// ユーザー
        /// </summary>
        public string User { get; }

        /// <summary>
        /// パスワード
        /// </summary>
        public string Password { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="user">ユーザー</param>
        /// <param name="password">パスワード</param>
        private BasicAuthenticationHeaderValue(string user, string password)
        {
            User = user;
            Password = password;
        }


        /// <summary>
        /// ヘッダーの値をデコードする
        /// </summary>
        /// <param name="rawHeaderValue">HTTPヘッダの "Basic {HTTP_ENCODED_VALUE}" の部分</param>
        /// <returns>ヘッダの値</returns>
        public static BasicAuthenticationHeaderValue? Decode(string rawHeaderValue)
        {
            if (!BasicHeaderValueRegex.IsMatch(rawHeaderValue))
            {
                return null;
            }

            var encodedValue = BasicHeaderValueRegex.Replace(rawHeaderValue, "$1");
            var decodedValue = Encoding.UTF8.GetString(Convert.FromBase64String(encodedValue));

            var userAndPassword = decodedValue.Split(':', 2);
            if (userAndPassword.Length != 2)
            {
                return null;
            }

            var user = userAndPassword[0];
            var password = userAndPassword[1];
            return new BasicAuthenticationHeaderValue(user, password);
        }
    }
}