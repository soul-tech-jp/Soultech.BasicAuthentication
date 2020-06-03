using System;
using System.Threading.Tasks;

namespace Soultech.BasicAuthentication.Internal
{
    /// <summary>
    /// ユーザー検索
    /// </summary>
    /// <typeparam name="TUser">検索対象のユーザーモデル</typeparam>
    public class UserFinder<TUser> where TUser : class
    {
        /// <summary>
        /// 検索処理使用フラグ
        /// </summary>
        public bool Used { get; }
        
        /// <summary>
        /// 検索処理
        /// </summary>
        public Func<string, Task<TUser>> Find { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="used">使用フラグ</param>
        /// <param name="find">検索処理</param>
        public UserFinder(bool used, Func<string, Task<TUser>> find)
        {
            Used = used;
            Find = find;
        }
    }
}