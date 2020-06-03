namespace Soultech.BasicAuthentication
{
    /// <summary>
    /// BASIC二人称のオプション
    /// </summary>
    /// <example>
    /// // Startup.cs
    /// public void ConfigureServices(IServiceCollection services)
    /// {
    ///     services.Configure&lt;BasicAuthenticationOptions&gt;(options =>
    ///     {
    ///         // Basic認証の user 部分を IdentityUser.Name として認証する場合
    ///         options.FindsByName = true;
    ///         // Basic認証の user 部分を IdentityUser.Email として認証する場合
    ///         options.FindsByEmail = true;
    ///         // Basic 印象の user 部分を IdentityUser.Id として認証する場合
    ///         options.FindsById = true;
    ///     });
    /// }
    /// </example>
    public class BasicAuthenticationOptions
    {
        /// <summary>
        /// BASIC認証ヘッダ Authorization: Basic {HEADER_VALUE}
        /// <br/>
        /// (HEADER_VALUE := user:password)
        /// <br/>
        /// の user を ユーザーのE-mailとして使用する
        /// </summary>
        /// <remarks>
        /// 複数の <c>FindsXxx</c> フラグが <c>true</c> の場合は、 <c>Email</c>, <c>Id</c>, <c>Name</c> の順にチェックを行い、
        /// 最初に一致したユーザーを認証に利用する
        /// </remarks>
        public bool FindsByEmail { get; set; }

        /// <summary>
        /// BASIC認証ヘッダ Authorization: Basic {HEADER_VALUE}
        /// <br/>
        /// (HEADER_VALUE := user:password)
        /// <br/>
        /// の user を ユーザーのIdとして使用する
        /// </summary>
        /// <remarks>
        /// 複数の <c>FindsXxx</c> フラグが <c>true</c> の場合は、 <c>Email</c>, <c>Id</c>, <c>Name</c> の順にチェックを行い、
        /// 最初に一致したユーザーを認証に利用する
        /// </remarks>
        public bool FindsById { get; set; }
        
        /// <summary>
        /// BASIC認証ヘッダ Authorization: Basic {HEADER_VALUE}
        /// <br/>
        /// (HEADER_VALUE := user:password)
        /// <br/>
        /// の user を ユーザーのNameとして使用する
        /// </summary>
        /// <remarks>
        /// 複数の <c>FindsXxx</c> フラグが <c>true</c> の場合は、 <c>Email</c>, <c>Id</c>, <c>Name</c> の順にチェックを行い、
        /// 最初に一致したユーザーを認証に利用する
        /// </remarks>
        public bool FindsByName { get; set; }
    }
}