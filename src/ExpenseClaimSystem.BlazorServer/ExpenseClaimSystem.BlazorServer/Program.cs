using ExpenseClaimSystem.BlazorServer.Components;
using System.Net.Http;
using ExpenseClaimSystem.Domain.Entities;
using ExpenseClaimSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ExpenseClaimSystem.Application.Interfaces;
using ExpenseClaimSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Register HttpClient for Blazor components (Blazor Server host provides a client instance)
// Default to the development HTTPS launch URL so API calls from the client hit the local server port

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


// Add API controllers for server-side API endpoints
builder.Services.AddControllers();

// Register application services
builder.Services.AddScoped<IAuthService, AuthService>();


// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

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

app.UseAntiforgery();

// Map API controllers
app.MapControllers();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ExpenseClaimSystem.BlazorServer.Client._Imports).Assembly);

app.Run();
