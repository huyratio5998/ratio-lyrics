using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ratio_Lyrics.Web.Data;
using Ratio_Lyrics.Web.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//Logger
var seriLogConfig = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(seriLogConfig);

//distributed cache
builder.Services.AddDistributedMemoryCache();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["RedisURL"];
});

// Add services to the container.
builder.Services.AddPaymentDemoDBContext(builder.Configuration);
builder.Services.AddFluentValidationConfig();
builder.Services.AddConfigurationAutoMapper();
builder.Services.AddApplicationRepositoriesConfig();
builder.Services.AddApplicationServicesConfig();
builder.Services.AddHttpClientFactoryConfig();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<RatioLyricsDBContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

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
