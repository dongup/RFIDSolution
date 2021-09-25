using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaiyoshaEPE.WebClient.Services
{
    public class NavigationService
    {
        private IJSRuntime js;

        public NavigationService(IJSRuntime js)
        {
            this.js = js;
        }

        public async Task NavigateBack()
        {
            await js.InvokeVoidAsync("goback");
        }

    }
}
