# ASP.NET Core BASIC認証ミドルウェア

![.NET Core](https://github.com/soul-tech-jp/Soultech.BasicAuthentication/workflows/.NET%20Core/badge.svg)

## 依存関係

* Microsoft.AspNetCore.Identity
* Microsoft.AspNetCore

## インストール方法

NuGet でインストールできます。

```
Install-Package Soultech.BasicAuthentication
```

## 使い方

`Startup.cs`で以下のようにオプションを追加する

```c#

 public void ConfigureServices(IServiceCollection services)
{
    services.Configure<BasicAuthenticationOptions>(options => {
        // Basic認証の user 部分を IdentityUser.Name として認証する場合
        options.FindsByName = true;
        // Basic認証の user 部分を IdentityUser.Email として認証する場合
        options.FindsByEmail = true;
        // Basic 印象の user 部分を IdentityUser.Id として認証する場合
        options.FindsById = true;
    });

    // Microsoft.AspNetCore.Identity に依存するので以下のように Identity も使用可能にすること！
    // 本ミドルウェアは User の型は任意の型を使用可能。
    services.AddIdentity<ApplicationUser, ApplicationRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();
}

```

`Startup.cs` で以下のようにミドルウェアを使用する。

```c#
 public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // ユーザーの型は、ミドルウェアを使用するアプリのユーザーの型を指定する
    // ※ app.UseAuthorization(); よりも前でコールすること！
    app.UseBasicAuthentication<IdentityUser>();

    app.UseAuthorization();
    app.UseAuthorization();
}
```

## 認証結果

通常の ClaimsPrincipal のように `HttpContext.User` で参照可能。

```c#
public IActionResult MyAction()
{
    return new JsonResult(HttpContext.User.Identity.Name);
}
```

`[Authorize]` 等の属性も利用可能

```c#
// Basic 認証に失敗した場合は 404 not found になる
[Authorize]
public IActionResult MyAction()
{
    return new JsonResult(HttpContext.User.Identity.Name);
}

```

