using ExpenseClaimSystem.Application.Services;
using ExpenseClaimSystem.BlazorServer.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

// Use an HTTP-based implementation of IExpenseClaimService on the WebAssembly client
builder.Services.AddScoped<IExpenseClaimService, HttpExpenseClaimService>();

await builder.Build().RunAsync();
