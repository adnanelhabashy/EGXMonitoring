using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Globalization;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Telerik.Blazor.Components.Editor;

namespace EGXMonitoring.Client
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _http;
        private readonly NavigationManager _navigationManager;

        public CustomAuthStateProvider(ILocalStorageService localStorageService, HttpClient http, NavigationManager navigationManager)
        {
            _localStorageService = localStorageService;
            _http = http;
            _navigationManager = navigationManager;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string authToken = await _localStorageService.GetItemAsStringAsync("authToken");

            var identity = new ClaimsIdentity();
            _http.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(authToken))
            {
                try
                {
                    var claims = ParseClaimsFromJwt(authToken);
                    identity = new ClaimsIdentity(claims, "jwt");
                    var isAdmin = IsAdmin(claims);
                    if (isAdmin)
                    {
                        // Add the admin claim to the identity
                        identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
                    }
                    if (!await _localStorageService.ContainKeyAsync("expiry"))
                    {
                        await _localStorageService.SetItemAsStringAsync("expiry", GetExpiryDateTime(claims));

                    }
                    DateTime expiry ;
                    string format = "dd/MM/yyyy hh:mm:ss tt";
                    var tokenExpiry = await _localStorageService.GetItemAsStringAsync("expiry");
                    tokenExpiry = tokenExpiry.Replace("ص", "AM");
                    tokenExpiry = tokenExpiry.Replace("م", "PM");
                    DateTime.TryParseExact(tokenExpiry, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out expiry);
                    if (DateTime.Now > expiry)
                    {
                        await _localStorageService.RemoveItemAsync("expiry");
                        await _localStorageService.RemoveItemAsync("authToken");
                        _navigationManager.NavigateTo("/login");
                    }

                    _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken.Replace("\"", ""));

                }
                catch
                {
                    await _localStorageService.RemoveItemAsync("authToken");
                    identity = new ClaimsIdentity();
                }
            }

            var user = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(user);

            NotifyAuthenticationStateChanged(Task.FromResult(state));
            return state;
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
        private IEnumerable<Claim> ParseClaimsFromJwt(string authToken)
        {
            var payload = authToken.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var KeyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            var claims = KeyValuePairs.Select(kv => new Claim(kv.Key, kv.Value.ToString()));
            return claims;
        }

        private bool IsAdmin(IEnumerable<Claim> claims)
        {
            return claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        }

        private string GetExpiryDateTime(IEnumerable<Claim> claims)
        {
            Claim expirationClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Expiration);
            return expirationClaim?.Value;
        }

    }
}
