using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RFIDSolution.WebAdmin.Services
{
    public class JsService
    {
        private IJSRuntime js;

        public JsService(IJSRuntime js)
        {
            this.js = js;
        }

        public async Task DownLoadFile(string url, string fileName = "")
        {
            await js.InvokeVoidAsync("downloadFromUrl", url, fileName);
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
