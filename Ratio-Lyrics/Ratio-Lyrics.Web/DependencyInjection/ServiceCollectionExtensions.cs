using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Ratio_Lyrics.Web.Configurations.Mapper;
using Ratio_Lyrics.Web.Data;
using Ratio_Lyrics.Web.Models;
using Ratio_Lyrics.Web.Repositories.Abstracts;
using Ratio_Lyrics.Web.Repositories.Implements;
using Ratio_Lyrics.Web.Services.Abstraction;
using Ratio_Lyrics.Web.Services.Abstractions;
using Ratio_Lyrics.Web.Services.Implements;
using Serilog;

namespace Ratio_Lyrics.Web.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurationAutoMapper(this IServiceCollection services)
        {
            return services.AddAutoMapper(typeof(ServiceProfile));
        }

        public static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<ArtistViewModelValidator>();
            services.AddValidatorsFromAssemblyContaining<MediaPlatformViewModelValidator>();
            services.AddValidatorsFromAssemblyContaining<SongMediaPlatformViewModelValidator>();
            services.AddValidatorsFromAssemblyContaining<SongViewModelValidator>();

            return services;
        }

        public static IServiceCollection AddApplicationServicesConfig(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ICommonService, CommonService>();
            //services.AddHostedService<RunUpdateViewsBackgroundTask>();

            services.AddScoped<IArtistService, ArtistService>();
            services.AddScoped<IMediaPlatformService, MediaPlatformService>();
            services.AddScoped<ISongService, SongService>();            
            services.AddScoped<ISiteSettingService, SiteSettingService>();
            services.AddScoped<ICacheService, CacheService>();

            return services;
        }

        public static IServiceCollection AddApplicationRepositoriesConfig(this IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static void AddRatioLyricsDBContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<RatioLyricsDBContext>(opt =>
            {
                opt.UseSqlServer(connectionString,
                    builder => builder.MigrationsAssembly(typeof(RatioLyricsDBContext).Assembly.FullName));

                opt.EnableSensitiveDataLogging();
            });

            services.AddDatabaseDeveloperPageExceptionFilter();
        }

        public static IServiceCollection AddHttpClientFactoryConfig(this IServiceCollection services)
        {
            services.AddHttpClient();
            //services.AddHttpClient(PaymentProvider.Paypal.ToString(), x =>
            //{
            //    x.BaseAddress = new Uri("");
            //});

            //services.AddHttpClient(PaymentProvider.Adyen.ToString(), x =>
            //{
            //    x.BaseAddress = new Uri("");
            //});

            return services;
        }

        public static WebApplicationBuilder AddSerilogConfig(this WebApplicationBuilder builder)
        {
            var seriLogConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(seriLogConfig);
            return builder;
        }
    }
}
