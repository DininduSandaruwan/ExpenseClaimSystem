using ExpenseClaimSystem.Application.Interfaces;
using ExpenseClaimSystem.Application.Services;
using ExpenseClaimSystem.BlazorServer.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization(); 
builder.Services.AddFluentUIComponents();

// Use an HTTP-based implementation of IExpenseClaimService on the WebAssembly client
builder.Services.AddScoped<IExpenseClaimService, HttpExpenseClaimService>();
// Register HTTP-based authentication service for the WebAssembly client
builder.Services.AddScoped<IAuthService, HttpAuthService>();

await builder.Build().RunAsync();
