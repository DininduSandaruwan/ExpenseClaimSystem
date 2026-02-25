using ExpenseClaimSystem.Application.Interfaces;
using ExpenseClaimSystem.Application.Services;
using ExpenseClaimSystem.BlazorServer.Components;
using ExpenseClaimSystem.Domain.Entities;
using ExpenseClaimSystem.Infrastructure.Data;
using ExpenseClaimSystem.Infrastructure.Repositories;
using ExpenseClaimSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ExpenseClaimSystem.Infrastructure;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Register your services here
builder.Services.AddScoped<IAuthService, AuthService>(); 
builder.Services.AddScoped<IExpenseClaimRepository, ExpenseClaimRepository>();
builder.Services.AddScoped<IExpenseClaimService, ExpenseClaimService>();
builder.Services.AddInfrastructure();
// Ensure an HttpClient with a sensible BaseAddress is available to interactive components
// (Blazor WebAssembly components executed during server prerendering need a BaseAddress)
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped(sp =>
{
    var accessor = sp.GetRequiredService<IHttpContextAccessor>();
    var request = accessor.HttpContext?.Request;

    if (request != null)
    {
        var uri = new UriBuilder
        {
            Scheme = request.Scheme,
            Host = request.Host.Host,
            Port = request.Host.Port ?? (request.IsHttps ? 443 : 80),
            Path = request.PathBase.HasValue ? request.PathBase.Value : string.Empty
        }.Uri;

        return new HttpClient { BaseAddress = uri };
    }

    // Fallback: use configured BaseAddress or localhost
    var fallback = builder.Configuration["BaseAddress"] ?? "https://localhost:5001/";
    return new HttpClient { BaseAddress = new Uri(fallback) };
});

builder.Services.AddCascadingAuthenticationState();

// Enable controllers for server-side API endpoints
builder.Services.AddControllers();

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await DbSeeder.SeedAsync(userManager, roleManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.MapStaticAssets();

// Routing + authentication/authorization middleware must be configured so controllers
// can set cookies/headers during the HTTP response.
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Anti-forgery middleware must be added after authentication/authorization and
// after UseRouting, but before endpoints/controllers so it can validate tokens
// during the HTTP request processing.
app.UseAntiforgery();

// Map API controllers first so they run within the HTTP request lifecycle and can
// modify response headers (for auth cookies).
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ExpenseClaimSystem.BlazorClient._Imports).Assembly);

app.Run();
