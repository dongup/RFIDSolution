using Microsoft.JSInterop;
using RFIDSolution.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RFIDSolution.WebAdmin.Services
{
    public class JsService
    {
        private IJSRuntime js;
        private HttpClient _client;

        public JsService(IJSRuntime js, HttpClient client)
        {
            this.js = js;
            _client = client;
        }

        public async Task DownLoadFile(string url)
        {
            string fullUrl = $"reports/{url}";
            string file = await _client.GetStringAsync(fullUrl);
            await js.InvokeVoidAsync("downloadFromUrl", Program.RootApiUrl + file, Path.GetFileName(file));
        }

        public async Task SetTitle(string title)
        {
            await js.InvokeVoidAsync("setTitle", title);
        }

        public async Task PrintDiv(string id)
        {
            await js.InvokeVoidAsync("printDiv", "#" + id);
        }
    }
}
