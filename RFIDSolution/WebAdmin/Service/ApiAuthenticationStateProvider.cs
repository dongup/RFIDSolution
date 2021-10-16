using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using RFIDSolution.Shared;
using RFIDSolution.Shared.Models;
using RFIDSolution.WebAdmin.Models;
using RFIDSolution.WebAdmin.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace RFIDSolution.WebAdmin.Services
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public ApiAuthenticationStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(savedToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            Program.TokenHeader = new AuthenticationHeaderValue("bearer", savedToken);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);
            
            ClaimsPrincipal claims = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(savedToken), "jwt"));

            string departmentId = claims.Claims.FirstOrDefault(x => x.Type == UserClaim.DepartmentId)?.Value;
            departmentId = string.IsNullOrEmpty(departmentId) ? "0" : departmentId;

            UserService.UserName = claims.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

            return new AuthenticationState(claims);
        }

        public void MarkUserAsAuthenticated(UserModel user)
        {
            var claims = new[] { 
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(UserClaim.FullName, user.FullName),
                new Claim(UserClaim.Department, user.DepartmentName??""),
                new Claim(UserClaim.DepartmentId, user.DepartmentId?.ToString()??""),
            };

            var claimIdentity = new ClaimsIdentity(claims, "apiauth");
            var authenticatedUser = new ClaimsPrincipal(claimIdentity);

            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            
            NotifyAuthenticationStateChanged(authState);
        }

        public void MarkUserAsLoggedOut()
        {
            Program.TokenHeader = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;

            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

            if (roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                    foreach (var parsedRole in parsedRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
