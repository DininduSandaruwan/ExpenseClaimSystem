using ExpenseClaimSystem.Application.DTOs;
using ExpenseClaimSystem.Application.Interfaces;
using System.Net.Http.Json;

namespace ExpenseClaimSystem.BlazorServer.Client.Services;

public class HttpAuthService : IAuthService
{
    private readonly HttpClient _http;

    public HttpAuthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<(bool Succeeded, string[] Errors)> RegisterAsync(RegisterRequest request)
    {
        var resp = await _http.PostAsJsonAsync("api/Auth/register", request);
        if (resp.IsSuccessStatusCode)
        {
            return (true, Array.Empty<string>());
        }

        // Try to read errors as string[] otherwise fall back to plain message
        try
        {
            var errors = await resp.Content.ReadFromJsonAsync<string[]>();
            if (errors != null && errors.Length > 0)
                return (false, errors);
        }
        catch { }

        var text = await resp.Content.ReadAsStringAsync();
        return (false, new[] { string.IsNullOrWhiteSpace(text) ? resp.ReasonPhrase ?? "Registration failed" : text });
    }

    public async Task<bool> LoginAsync(LoginRequest request)
    {
        var resp = await _http.PostAsJsonAsync("api/Auth/login", request);
        return resp.IsSuccessStatusCode;
    }

    public async Task LogoutAsync()
    {
        // Call server logout endpoint if available
        try
        {
            await _http.PostAsync("api/Auth/logout", null);
        }
        catch { }
    }
}
