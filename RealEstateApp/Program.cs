using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using MudBlazor.Services;
using RealEstateApp.Components;
using RealEstateApp.Data;
using RealEstateApp.Data.Repositories;
using RealEstateApp.Models;
using RealEstateApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

// --- Data Protection түлхүүрийг тогтмол хавтсанд хадгалах ---
// IIS дээр app pool recycle/дахин deploy хийх бүрд Data Protection-ий
// түлхүүр санах ойд шинээр үүсдэг тул өмнө нь гаргасан antiforgery token
// (login маягт нээлттэй хэвээр байхад) болон auth cookie тэр даруй хүчингүй
// болж "A valid antiforgery token was not provided" алдаа, эсвэл нэвтэрсэн
// хэрэглэгчид гэнэт гарагддаг шалтгаан нь энэ. Түлхүүрийг deploy хийхэд
// дарагддаггүй тогтмол хавтсанд хадгалснаар энэ давтагдахгүй.
// Анхаар: доорх замыг сервер дээрээ бодит, IIS AppPool-ийн identity-д
// бичих эрхтэй, deploy хийхэд УСТГАГДАХГҮЙ хавтас руу тохируулна уу
// (жишээ нь: C:\dp-keys\RealEstateApp — deploy хийдэг wwwroot/site
// хавтаснаас ГАДУУР байх ёстой).
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\dp-keys\RealEstateApp"))
    .SetApplicationName("RealEstateApp");

// --- Нэвтрэлт ---
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.Cookie.Name = "RealEstateAuth";
        o.LoginPath = "/login";
        o.AccessDeniedPath = "/login";
        o.ExpireTimeSpan = TimeSpan.FromHours(8);
        o.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IPasswordHasher<AgentAccount>, PasswordHasher<AgentAccount>>();

// --- Cloudinary ---
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddSingleton<IImageStorageService, CloudinaryImageStorageService>();

// --- Өгөгдөл ---
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<ILookupRepository, LookupRepository>();
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IAgentRepository, AgentRepository>();
builder.Services.AddScoped<IPropertyImageRepository, PropertyImageRepository>();   
builder.Services.AddScoped<IClosureRepository, ClosureRepository>();
builder.Services.AddScoped<ISiteRepository, SiteRepository>();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ISiteBrandService, SiteBrandService>();
builder.Services.AddScoped<IViewingRepository, ViewingRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<ILocationAdminRepository, LocationAdminRepository>();
builder.Services.AddHttpClient();
builder.Services.Configure<TelegramSettings>(builder.Configuration.GetSection("Telegram"));
builder.Services.AddSingleton<ITelegramNotifier, TelegramNotifier>();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});

app.Run();