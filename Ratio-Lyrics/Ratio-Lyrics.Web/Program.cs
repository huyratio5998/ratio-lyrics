using Microsoft.AspNetCore.Identity;
using Ratio_Lyrics.Web.Data;
using Ratio_Lyrics.Web.DependencyInjection;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogConfig();

//distributed cache
builder.Services.AddDistributedMemoryCache();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["RedisURL"];
});
builder.Services.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>(c =>
{
    var config = builder.Configuration["RedisURL"];
    return ConnectionMultiplexer.Connect(config);
});

// Add services to the container.
builder.Services.AddRatioLyricsDBContext(builder.Configuration);
builder.Services.AddFluentValidationConfig();
builder.Services.AddConfigurationAutoMapper();
builder.Services.AddApplicationRepositoriesConfig();
builder.Services.AddApplicationServicesConfig();
builder.Services.AddHttpClientFactoryConfig();
builder.Services.AddGoogleCaptchaConfig(builder.Configuration);

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<RatioLyricsDBContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapDefaultControllerRoute();
app.MapRazorPages();

app.Run();
