using ExpenseClaimSystem.Application.DTOs;
using ExpenseClaimSystem.Application.Services;
using ExpenseClaimSystem.Domain.Entities;
using System.Net.Http.Json;

namespace ExpenseClaimSystem.BlazorServer.Client.Services;

public class HttpExpenseClaimService : IExpenseClaimService
{
    private readonly HttpClient _http;

    public HttpExpenseClaimService(HttpClient http)
    {
        _http = http;
    }

    public async Task<Guid> CreateAsync(CreateExpenseClaimDto dto, string userId)
    {
        // userId can be included in the DTO or as a header/query if needed by the server
        var response = await _http.PostAsJsonAsync("api/ExpenseClaims", dto);
        response.EnsureSuccessStatusCode();

        // Expect server to return the created id as GUID in the response body
        var created = await response.Content.ReadFromJsonAsync<Guid>();
        return created;
    }

    public async Task<List<ExpenseClaim>> GetFilteredAsync(ExpenseClaimFilterDto filter)
    {
        var response = await _http.PostAsJsonAsync("api/ExpenseClaims/filter", filter);
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<List<ExpenseClaim>>();
        return list ?? new List<ExpenseClaim>();
    }
}
