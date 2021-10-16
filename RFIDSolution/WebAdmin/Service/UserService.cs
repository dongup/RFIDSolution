using RFIDSolution.Shared;
using RFIDSolution.Shared.DTO;
using RFIDSolution.Shared.Models;
using RFIDSolution.Shared.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace RFIDSolution.WebAdmin.Service
{
    public class UserService
    {
        private readonly DialogService _dialog;
        private readonly HttpClient _client;

        public UserService(DialogService dialog, HttpClient http)
        {
            _dialog = dialog;
            _client = http;
        }

        public static async Task<UserModel> getCurrentUser()
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(Program.ApiUrl),
            };
            client.DefaultRequestHeaders.Authorization = Program.TokenHeader;

            UserModel user = new UserModel();

            var rspns = await client.GetFromJsonAsync<ResponseModel<UserModel>>($"users/byUserName/{UserName}");
            user = rspns.Result;
            if (!rspns.IsSuccess)
            {
                Console.WriteLine(rspns.Message);
            }
            return user;
        }

        public static string UserName { get; set; }

        public async Task<UserModel> GetUserById(int id)
        {
            var rspns = await _client.GetFromJsonAsync<ResponseModel<UserModel>>($"users/{id}");
            if (rspns.IsSuccess)
            {
                return rspns.Result;
            }
            else
            {
                _dialog.ErrorAlert(rspns.Message);
                return new UserModel();
            }
        }

        public async Task<List<RoleModel>> GetRoles()
        {
            var rspns = await _client.GetFromJsonAsync<ResponseModel<List<RoleModel>>>($"roles");
            if (rspns.IsSuccess)
            {
                return rspns.Result;
            }
            else
            {
                _dialog.ErrorAlert(rspns.Message);
                return new List<RoleModel>();
            }
        }

        public async Task<bool> ConfirmPassword(string password)
        {
            bool result = false;
            var rspns = await _client.GetFromJsonAsync<ResponseModel<bool>>($"Authenticate/confirmpassword/{password}");
            if (rspns.IsSuccess)
            {
                result = rspns.Result;
            }
            else
            {
                await _dialog.ErrorAlert(rspns.Message);
            }
            return result;
        }

        public async Task<bool> ResetPassword(int userId, string password)
        {
            bool result = false;
            var req = await _client.PostAsJsonAsync<object>($"ResetPassword/{userId}", new { NewPassword = password });
            var rspns = await req.Content.ReadFromJsonAsync<ResponseModel<bool>>();
            if (rspns.IsSuccess)
            {
                result = rspns.Result;
            }
            else
            {
                _dialog.ErrorAlert(rspns.Message);
            }
            return result;
        }
    }
}
