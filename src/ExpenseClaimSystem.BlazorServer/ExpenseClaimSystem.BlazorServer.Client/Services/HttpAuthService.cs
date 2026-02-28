using ExpenseClaimSystem.Application.DTOs;
using ExpenseClaimSystem.Application.Interfaces;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ExpenseClaimSystem.BlazorServer.Client.Services;

public class HttpAuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    public HttpAuthService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
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

    public class LoginResponse { public string Token { get; set; } = string.Empty; }

    public async Task<string?> LoginAsync(LoginRequest request)
    {
        var resp = await _http.PostAsJsonAsync("api/Auth/login", request);
        if (!resp.IsSuccessStatusCode)
            return null;

        try
        {
            var result = await resp.Content.ReadFromJsonAsync<LoginResponse>();
            var token = result?.Token ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(token))
            {
                // store token in localStorage and set default header
                try { await _js.InvokeVoidAsync("localStorage.setItem", "authToken", token); } catch { }
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return token;
            }
        }
        catch { }

        return null;
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
        }
        catch { }

        if (_http.DefaultRequestHeaders.Authorization != null)
            _http.DefaultRequestHeaders.Authorization = null;

        // Optionally call server logout endpoint if you maintain server-side sessions
        try
        {
            await _http.PostAsync("api/Auth/logout", null);
        }
        catch { }
    }
}
