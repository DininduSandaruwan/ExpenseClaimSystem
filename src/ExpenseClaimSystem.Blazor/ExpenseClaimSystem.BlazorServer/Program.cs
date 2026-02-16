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
builder.Services.AddHttpClient();

builder.Services.AddCascadingAuthenticationState();

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

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ExpenseClaimSystem.BlazorClient._Imports).Assembly);

app.UseAuthentication();
app.UseAuthorization();

app.Run();
