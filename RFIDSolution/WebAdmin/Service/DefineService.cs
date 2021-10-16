using RFIDSolution.Shared.Models;
using RFIDSolution.Shared.Models.Products;
using RFIDSolution.Shared.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace RFIDSolution.WebAdmin.Service
{
    public class DefineService
    {
        private HttpClient _httpClient;
        private DialogService _dialog;

        public DefineService(HttpClient client, DialogService dialog)
        {
            _httpClient = client;
            _dialog = dialog;
        }

        public async Task<List<ModelResponse>> GetModels(string keyWord = "")
        {
            var models = new List<ModelResponse>();
            var rspns = await _httpClient.GetFromJsonAsync<ResponseModel<List<ModelResponse>>>("model");
            if (rspns.IsSuccess)
            {
                models = rspns.Result;
            }
            else
            {
                _dialog.ErrorAlert(rspns.Message);
            }
            return models;
        }

        public async Task<List<CategoryResponse>> GetCategories(string keyWord = "")
        {
            var models = new List<CategoryResponse>();
            var rspns = await _httpClient.GetFromJsonAsync<ResponseModel<List<CategoryResponse>>>("category");
            if (rspns.IsSuccess)
            {
                models = rspns.Result;
            }
            else
            {
                _dialog.ErrorAlert(rspns.Message);
            }
            return models;
        }

        public async Task<List<DepartmentResponse>> GetDepartmnets(string keyWord = "")
        {
            var models = new List<DepartmentResponse>();
            var rspns = await _httpClient.GetFromJsonAsync<ResponseModel<List<DepartmentResponse>>>("department");
            if (rspns.IsSuccess)
            {
                models = rspns.Result;
            }
            else
            {
                _dialog.ErrorAlert(rspns.Message);
            }
            return models;
        }

        public async Task<string> GetNewTransferDocumentCode()
        {
            string str = "";
            var rspns = await _httpClient.GetFromJsonAsync<ResponseModel<string>>("ProductTransfers/documentcode");
            if (rspns.IsSuccess)
            {
                str = rspns.Result;
            }
            else
            {
                _dialog.ErrorAlert(rspns.Message);
            }
            return str;
        }
    }
}
