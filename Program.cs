using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using OpenAI.Interfaces;
using OpenAI.Managers;
using QuanLySoTietKiem.BackgroundServices;
using QuanLySoTietKiem.BackgroundServices.Interfaces;
using QuanLySoTietKiem.Configurations;
using QuanLySoTietKiem.Constaints;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Repositories;
using QuanLySoTietKiem.Repositories.Implementations;
using QuanLySoTietKiem.Repositories.Interfaces;
using QuanLySoTietKiem.Services;
using QuanLySoTietKiem.Services.Implementations;
using QuanLySoTietKiem.Services.Interfaces;
using Services.Implementations;
using Services.Interfaces;
using Amazon.S3;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VinaHostConnectionV2")));
builder.Services.AddHealthChecks();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.SignIn.RequireConfirmedEmail = true;
    options.User.RequireUniqueEmail = true;

    // Cấu hình Two-Factor Authentication
    options.Tokens.AuthenticatorTokenProvider = "Email";
    options.Tokens.AuthenticatorIssuer = "QuanLySoTietKiem";
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

#region  Add Services 

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ILaiSuatDuKienService, LaiSuatDuKienService>();
builder.Services.AddScoped<ITienIchService, TienIchService>();
builder.Services.AddScoped<IAutoUpdateMoneyService, AutoUpdateMoneyServiceImplementation>();
builder.Services.AddScoped<IVNPayService, VNPayService>();
builder.Services.AddScoped<IPayPalService, PayPalService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISavingAccountService, SavingAccountService>();
builder.Services.AddScoped<IOpenAIService>(sp =>
    new OpenAIService(new OpenAI.OpenAiOptions() { ApiKey = builder.Configuration["OpenAI:ApiKey"] }));
builder.Services.AddScoped<IFinancialAdvisorService, FinancialAdvisorService>();
builder.Services.AddScoped<ITwoFactorAuthenticationService, TwoFactorAuthenticationService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IInterestRateRepository, InterestRateRepository>();
builder.Services.AddSingleton<S3Service>();
builder.Services.AddScoped<IRoleService, RoleService>();

#endregion

#region  Add Host Services
builder.Services.AddHostedService<AutoUpdateMoneyService>();
builder.Services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
builder.Services.AddScoped<IUrlHelper>(provider =>
{
    var actionContext = provider.GetRequiredService<IActionContextAccessor>().ActionContext;
    return provider.GetRequiredService<IUrlHelperFactory>().GetUrlHelper(actionContext);
});
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
#endregion

#region Add Repositories

builder.Services.AddScoped<ISavingAccountTypeRepository, SavingAccountTypeRepository>();
builder.Services.AddScoped<ILaiSuatDuKienRepository, LaiSuatDuKienRepository>();
builder.Services.AddScoped<ILaiSuatDuKienRepository, LaiSuatDuKienRepository>();
builder.Services.AddScoped<ISavingAccountTypeRepository, SavingAccountTypeRepository>();
builder.Services.AddScoped<ISavingAccountRepository, SavingAccountRepository>();
builder.Services.AddScoped<ITwoFactorAuthenticationRepository, TwoFactorAuthenticationRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IPaymentHistoryRepository, PaymentHistoryRepository>();
#endregion

#region Config Settings 

// Cấu hình Google OAuth
builder.Services.Configure<GoogleAuthSettings>(builder.Configuration.GetSection("GoogleAuth"));
builder.Services.AddHttpClient();
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();

// Update the EmailSettings configuration to map property names correctly
builder.Services.Configure<EmailSettings>(options =>
{
    // First bind the section to get any properties that match directly
    builder.Configuration.GetSection("EmailSettings").Bind(options);

    // Then explicitly map properties with different names
    options.SmtpServer = builder.Configuration["EmailSettings:SmtpServer"] ?? options.SmtpServer;
    options.SmtpPort = int.TryParse(builder.Configuration["EmailSettings:Port"], out int port) ? port : options.SmtpPort;
    options.SmtpUsername = builder.Configuration["EmailSettings:Username"] ?? options.SmtpUsername;
    options.SmtpPassword = builder.Configuration["EmailSettings:Password"] ?? options.SmtpPassword;
    options.FromEmail = builder.Configuration["EmailSettings:SenderEmail"] ?? options.FromEmail;
    options.FromName = builder.Configuration["EmailSettings:SenderName"] ?? options.FromName;
    options.EnableSsl = bool.TryParse(builder.Configuration["EmailSettings:EnableSsl"], out bool enableSsl) ? enableSsl : options.EnableSsl;
});


#endregion




#region Config Dapper

builder.Services.AddSingleton<DapperContext>();

#endregion 

#region Config Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyConstants.RequireAdmin, policy => policy.RequireRole(RoleConstants.Admin));
    options.AddPolicy(PolicyConstants.RequireUser, policy => policy.RequireRole(RoleConstants.User));
    options.AddPolicy(PolicyConstants.RequireAdminOrUser, policy => policy.RequireRole(RoleConstants.Admin, RoleConstants.User));
});
#endregion


//Config Logging 
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});
//seeder role 

using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Bắt đầu seed dữ liệu");
         context.Database.EnsureCreated();
        LoaiSoTietKiemSeeder.Seed(context);
        await AccountSeeder.SeedUsersAsync(userManager, roleManager);

        logger.LogInformation("Seed dữ liệu thành công");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Lỗi khi seed dữ liệu");
    }
}
builder.Services.AddHttpContextAccessor();

// Đăng ký HttpClient cho PayPal
builder.Services.AddHttpClient("PayPal", client =>
{
    // Cấu hình mặc định cho HttpClient
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});
//Redis 
// builder.Services.AddStackExchangeRedisCache(
//     options =>
//     {
//         options.Configuration = builder.Configuration["Redis:ConnectionString"];
//         options.InstanceName = builder.Configuration["Redis:InstanceName"];
//     }
// );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}
else
{
    // Trong môi trường phát triển, không bắt buộc HTTPS
    // Điều này giúp tránh lỗi SSL khi callback từ PayPal
    app.UseDeveloperExceptionPage();
}
app.MapHealthChecks("/health");
app.UseStatusCodePagesWithReExecute("/Error/{0}");
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();

