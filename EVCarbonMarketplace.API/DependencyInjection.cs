using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Repository.Implement;
using EVCarbonMarketplace.Repository.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using CloudinaryDotNet;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using EVCarbonMarketplace.Service.Interface;
using EVCarbonMarketplace.Service.Implement;
using StackExchange.Redis;
using EVCarbonMarketplace.Model.Payload.Settings;
using System.Text;
using FirebaseAdmin;
using Google.Cloud.Storage.V1;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Options;
namespace EVCarbonMarketplace.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork<EvcarbonMarketplaceContext>, UnitOfWork<EvcarbonMarketplaceContext>>();
            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            services.AddDbContext<EvcarbonMarketplaceContext>(options => options.UseSqlServer(GetConnectionString()));
            return services;
        }
        private static string CreateClientId(IConfiguration configuration)
        {
            var clientId = Environment.GetEnvironmentVariable("GOOGLE_OAUTH_CLIENT_ID")
                           ?? configuration.GetValue<string>("Oauth:ClientId");
            return clientId;
        }

        private static string CreateClientSecret(IConfiguration configuration)
        {
            var clientSecret = Environment.GetEnvironmentVariable("GOOGLE_OAUTH_CLIENT_SECRET")
                               ?? configuration.GetValue<string>("Oauth:ClientSecret");
            return clientSecret;
        }
        public static IServiceCollection AddJwtValidation(this IServiceCollection services)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = "EVCarBonMarketplace",
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Convert.FromHexString("0102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F00"))
                };
            }).AddCookie(
                options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.None;
                })
            .AddGoogle(options =>
            {
                options.ClientId = CreateClientId(configuration);
                options.ClientSecret = CreateClientSecret(configuration);
                options.SaveTokens = true;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Scope.Add("profile");
                options.ClaimActions.MapJsonKey("picture", "picture");

            });
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.Secure = CookieSecurePolicy.Always;
            }); ;
            ;
            return services;
        }

        private static string GetConnectionString()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", true, true)
                        .Build();
            var strConn = config["ConnectionStrings:DefaultDB"];

            return strConn;
        }
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IAuthenticateService, AuthenticateService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IGoogleAuthenticationService, GoogleAuthenticationService>();
            services.AddScoped<IOwnerService, OwnerService>();
            services.AddScoped<ICvaService, CvaService>();
            services.AddScoped<IElectricVehicleService, ElectricVehicleService>();
            services.AddScoped<ICarbonEmissionService, CarbonEmissionService>();
            services.AddScoped<IFileReaderService, FileReaderService>();
            services.AddScoped<ICarbonCreditService, CarbonCreditService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IVehicleTypeService, VehicleTypeService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<ICarbonListingService, CarbonListingService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IBidService, BidService>();
            services.AddScoped<IVehicleTelemetryService, VehicleTelemetryService>();
            services.AddScoped<IBankAccountService, BankAccountService>();
            services.AddScoped<IWithdrawService, WithdrawService>();
            services.AddScoped<IDisputeService, DisputeService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();
            services.AddScoped<ISystemSettingService, SystemSettingService>();
            services.AddScoped<ICertificateService, CertificateService>();
            services.AddScoped<INotificationService, NotificationService>();







            return services;
        }
        public static IServiceCollection AddHttpClientServices(this IServiceCollection services)
        {
            services.AddHttpClient();
            return services;
        }

        public static IServiceCollection AddLazyResolution(this IServiceCollection services)
        {
            services.AddTransient(typeof(Lazy<>), typeof(LazyResolver<>));
            return services;
        }
        private class LazyResolver<T> : Lazy<T> where T : class
        {
            public LazyResolver(IServiceProvider serviceProvider)
                : base(() => serviceProvider.GetRequiredService<T>())
            {
            }
        }

        public static IServiceCollection AddCloudinary(this IServiceCollection services, IConfiguration configuration)
        {
            var account = new CloudinaryDotNet.Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:Secret"]);

            var cloudinary = new Cloudinary(account)
            {
                Api = { Secure = true }
            };

            services.AddSingleton(account);
            services.AddSingleton(cloudinary);
            return services;
        }
        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConn = configuration.GetConnectionString("Redis")
                ?? throw new InvalidOperationException("Redis connection string not found");
            services.AddSingleton<IConnectionMultiplexer>(
                _ => ConnectionMultiplexer.Connect(redisConn));
            return services;
        }
    

        public static IServiceCollection AddFirebaseStorage(this IServiceCollection services, IConfiguration configuration)
        {
            var firebaseConfig = configuration.GetSection("Firebase").Get<FirebaseSettings>();

            if (string.IsNullOrEmpty(firebaseConfig.CredentialPath))
                throw new InvalidOperationException("Firebase:CredentialPath is missing in configuration.");

            if (!File.Exists(firebaseConfig.CredentialPath))
            {
                firebaseConfig.CredentialPath = Path.Combine(Directory.GetCurrentDirectory(), firebaseConfig.CredentialPath);
            }

            if (!File.Exists(firebaseConfig.CredentialPath))
                throw new FileNotFoundException("Không tìm thấy file Firebase credentials", firebaseConfig.CredentialPath);

            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(firebaseConfig.CredentialPath)
                });
            }

            var storageClient = StorageClient.Create(GoogleCredential.FromFile(firebaseConfig.CredentialPath));

            services.Configure<FirebaseSettings>(configuration.GetSection("Firebase"));
            services.AddSingleton(storageClient);

            return services;
        }
        public static IServiceCollection AddFirestore(this IServiceCollection services, IConfiguration configuration)
        {
            // Lấy settings chung
            services.Configure<FirebaseSettings>(configuration.GetSection("Firebase"));

            services.AddSingleton(sp =>
            {
                var opt = sp.GetRequiredService<IOptions<FirebaseSettings>>().Value;

                if (string.IsNullOrWhiteSpace(opt.ProjectId))
                    throw new InvalidOperationException("Firebase:ProjectId is missing.");
                    
                // Resolve CredentialPath giống AddFirebaseStorage
                var credPath = opt.CredentialPath;
                if (!File.Exists(credPath))
                    credPath = Path.Combine(Directory.GetCurrentDirectory(), credPath);
                if (!File.Exists(credPath))
                    throw new FileNotFoundException("Không tìm thấy file Firebase credentials", credPath);

                var credential = GoogleCredential.FromFile(credPath);

                // Tạo FirestoreDb với credential
                return new FirestoreDbBuilder
                {
                    ProjectId = opt.ProjectId,
                    Credential = credential
                }.Build();
            });

            return services;
        }

    }
}
