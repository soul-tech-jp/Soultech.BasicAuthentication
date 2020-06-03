using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Soultech.BasicAuthentication.Exceptions;
using Soultech.BasicAuthentication.Internal;

namespace Soultech.BasicAuthentication
{
    /// <summary>
    /// Basic認証を行うミドルウェア
    /// </summary>
    /// <example>
    /// </example>
    public class BasicAuthenticationMiddleware<TUser> where TUser : class
    {
        /// <summary>
        /// 次のリクエストハンドラ
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="next">次のリクエストハンドラ</param>
        public BasicAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// ミドルウェアの処理
        /// <br/>
        /// ここで、Authorizationヘッダがリクエストに含まれる場合は、サインインを行う。
        /// </summary>
        /// <remarks>
        /// この関数のシグネチャーはフレームワークで決まっているため、変更しないこと。
        /// 第2引数以降は、DIされるパラメータ
        /// </remarks>
        /// <param name="context">現在のコンテキスト</param>
        /// <returns><see cref="Task"/></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                await TryAuthenticate(context);
            }

            await _next(context);
        }

        /// <summary>
        /// 認証できる場合は、認証する
        /// </summary>
        /// <param name="context">HTTPコンテキスト</param>
        /// <returns><see cref="Task"/></returns>
        private async Task TryAuthenticate(HttpContext context)
        {
            try
            {
                var account = BasicAuthenticationHeaderValueExtractor.ExtractBasicAuthHeaderValue(context);
                await TrySignIn(account, context);
            }
            catch (Exception ex)
            {
                LogError(context, ex);
                throw ex is BasicAuthenticationException 
                    ? ex 
                    : new BasicAuthenticationException(ex.Message, ex);
            }
        }

        /// <summary>
        /// 可能な場合 SignIn する
        /// </summary>
        /// <param name="basicHeaderValue">BASICヘッダの値</param>
        /// <param name="context">HTTPコンテキスト</param>
        /// <returns><see cref="Task"/></returns>
        private async Task TrySignIn(BasicAuthenticationHeaderValue? basicHeaderValue, HttpContext context)
        {
            if (basicHeaderValue == null)
            {
                return;
            }

            var options = context.RequestServices.GetService<IOptions<BasicAuthenticationOptions>>();
            var userManager = context.RequestServices.GetService<UserManager<TUser>>();
            var signInManager = context.RequestServices.GetService<SignInManager<TUser>>();

            var user = await FindUser(options?.Value ?? new BasicAuthenticationOptions(), userManager, basicHeaderValue.User);

            if (user == null)
            {
                return;
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, basicHeaderValue.Password, false);
            if (!result.Succeeded)
            {
                return;
            }

            context.User = await signInManager.CreateUserPrincipalAsync(user);
        }

        /// <summary>
        /// ユーザーを検索する
        /// </summary>
        /// <param name="options">Basic認証のオプション</param>
        /// <param name="userManager">ユーザーマネージャ</param>
        /// <param name="userKey">ユーザーの検索用キー</param>
        /// <returns>
        /// 検索結果のユーザー <see cref="TUser"/>, 見つからない場合は <c>null</c>
        /// </returns>
        private static async Task<TUser?> FindUser(BasicAuthenticationOptions options,
            UserManager<TUser> userManager, string userKey)
        {
            var finders = new[]
            {
                new UserFinder<TUser>(options.FindsByEmail, userManager.FindByEmailAsync),
                new UserFinder<TUser>(options.FindsById, userManager.FindByIdAsync),
                new UserFinder<TUser>(options.FindsByName, userManager.FindByNameAsync)
            }.Where(x => x.Used).ToList();

            foreach (var finder in finders)
            {
                var user = await finder.Find(userKey);
                if (user != null)
                {
                    return user;
                }
            }

            return null;
        }
        
        /// <summary>
        /// エラーログを記録する
        /// </summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="ex">例外</param>
        private static void LogError(HttpContext context, Exception ex)
        {
            context.RequestServices?.GetService<ILogger<BasicAuthenticationMiddleware<TUser>>>()
                ?.LogError(ex, "Basic Auth Failed");
        }
    }
}