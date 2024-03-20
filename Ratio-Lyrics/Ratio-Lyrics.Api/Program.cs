using Asp.Versioning;
using Microsoft.Extensions.Options;
using Ratio_Lyrics.Api.ServiceConfiguration;
using Ratio_Lyrics.Api.SwaggerConfig;
using Ratio_Lyrics.Web.DependencyInjection;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddControllers();

builder.AddSerilogConfig();
builder.Services.AddRatioLyricsDBContext(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.AddFluentValidationConfig();
builder.Services.AddConfigurationAutoMapper();
builder.Services.AddApplicationRepositoriesConfig();
builder.Services.AddApplicationServicesConfig();
builder.Services.AddHttpClientFactoryConfig();

//enable cors
builder.Services.AddCors(setup =>
{
    setup.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.WithOrigins(builder.Configuration["AllowCorsCall"] ?? "https://localhost:7206")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
    });
});
//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<SwaggerDefaultValues>();
});
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1.0);
    opt.ReportApiVersions = true;
    opt.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.SubstituteApiVersionInUrl = true;
    options.GroupNameFormat = "'v'VVV";
    options.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();

        foreach (var description in descriptions)
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName;
            options.SwaggerEndpoint(url, name);
        }
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
