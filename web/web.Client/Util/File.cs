using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Forms;

namespace web.Client.Util
{
    public class File
    {
        public async static Task SaveAs(IJSRuntime js, string filename, string text)
        {
            var data = Encoding.UTF8.GetBytes(text);

            await js.InvokeAsync<object>(
                "saveAsFile",
                filename,
                Convert.ToBase64String(data));
        }

        public async static Task<string> InputFileToString(InputFileChangeEventArgs eventArgs)
        {
            MemoryStream ms = new MemoryStream();
            await eventArgs.File.OpenReadStream().CopyToAsync(ms);
            var bytes = ms.ToArray();
            var str = Encoding.UTF8.GetString(bytes);
            return str;
        }
    }
}
