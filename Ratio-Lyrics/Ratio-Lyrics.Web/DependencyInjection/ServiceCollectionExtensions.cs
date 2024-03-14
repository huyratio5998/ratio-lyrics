using Microsoft.EntityFrameworkCore;
using Ratio_Lyrics.Web.Configurations.Mapper;
using Ratio_Lyrics.Web.Data;
using Ratio_Lyrics.Web.Repositories.Abstracts;
using Ratio_Lyrics.Web.Repositories.Implements;
using Ratio_Lyrics.Web.Services.Abstractions;
using Ratio_Lyrics.Web.Services.Implements;

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
            //return services.AddValidatorsFromAssemblyContaining<ProductViewModelValidator>();
            return services;
        }

        public static IServiceCollection AddApplicationServicesConfig(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ICommonService, CommonService>();


            return services;
        }

        public static IServiceCollection AddApplicationRepositoriesConfig(this IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static void AddPaymentDemoDBContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<RatioLyricsDBContext>(opt => opt.UseSqlServer(connectionString,
                builder => builder.MigrationsAssembly(typeof(RatioLyricsDBContext).Assembly.FullName)));

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
    }
}
