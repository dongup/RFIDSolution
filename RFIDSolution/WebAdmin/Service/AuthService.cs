using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using RFIDSolution.Shared.Models;
using RFIDSolution.Shared.Models.Indentity;
using RFIDSolution.WebAdmin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace RFIDSolution.WebAdmin.Service
{
    public class AuthService : IAuthService
    {
        private HttpClient _httpClient;
        private ILocalStorageService _localStorage;
        private ApiAuthenticationStateProvider _stateProvider;

        public AuthService(HttpClient client, ILocalStorageService localStorage, AuthenticationStateProvider stateProvider)
        {
            _httpClient = client;
            _localStorage = localStorage;
            _stateProvider = (ApiAuthenticationStateProvider)stateProvider;
        }

        public async Task<ResponseModel<LoginResponseModel>> Login(LoginModel value)
        {
            LoginResponseModel loginResponse = new LoginResponseModel();

            var httpReq = await _httpClient.PostAsJsonAsync<LoginModel>("login", value);
            var rspns = await httpReq.Content.ReadFromJsonAsync<ResponseModel<LoginResponseModel>>();

            if (rspns.IsSuccess)
            {
                await _localStorage.SetItemAsync<String>("authToken", rspns.Result.Token);
                _stateProvider.MarkUserAsAuthenticated(rspns.Result.User.UserName);
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", rspns.Result.Token);
            }

            return rspns;
        }


        public async Task LogOut()
        {
            await _localStorage.RemoveItemAsync("authToken");
            _stateProvider.MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
