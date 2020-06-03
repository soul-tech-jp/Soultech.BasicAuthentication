using Microsoft.AspNetCore.Builder;

namespace Soultech.BasicAuthentication
{
    /// <summary>
    /// <see cref="IApplicationBuilder"/>用の拡張クラス
    /// </summary>
    public static class BasicAuthenticationMiddlewareExtension
    {
        /// <summary>
        /// <see cref="IApplicationBuilder"/>用の拡張メソッド
        /// <br/>
        /// <c>Startup.Configure</c>メソッド内で使用すること。
        /// </summary>
        /// <param name="builder">アプリケーションビルダー</param>
        /// <typeparam name="TUser">アプリケーションが使用するユーザークラスの方</typeparam>
        /// <returns>アプリケーションビルダー</returns>
        /// <remarks>
        /// <see cref="UseBasicAuthentication{TUser}"/> は UseAuthorization, UseAuthentication の前に呼び出すこと!
        /// <br/>
        /// <see cref="UseBasicAuthentication{TUser}"/> を UseAuthorization, UseAuthentication よりも後で呼び出した場合、
        /// コントローラクラスでの <see cref="Microsoft.AspNetCore.Authorization.AuthorizeAttribute"/> 等が正常に動作しない。
        /// </remarks>
        /// <example>
        ///
        /// // UseBasicAuthentication は UseAuthorization, UseAuthentication の前に呼び出す。
        /// public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Seeder seeder)
        /// {
        ///     // ミドルウェアの設定 ....
        ///
        ///     app.UseBasicAuthentication&lt;ApplicationUser&gt;();
        ///     app.UseAuthentication();
        ///     app.UseAuthorization();
        ///     
        ///     // ミドルウェアの設定 ....
        /// 
        ///     app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        /// } 
        ///
        /// </example>
        public static IApplicationBuilder UseBasicAuthentication<TUser>(this IApplicationBuilder builder) 
            where TUser : class
        {
            return builder.UseMiddleware<BasicAuthenticationMiddleware<TUser>>();
        }
    }
}