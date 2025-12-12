using DashLib.DankAPI;
using DashLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashLib.API
{
    public class MailAPI : IMailAPI
    {
        private readonly HttpClient _httpClient;
        private string _mailBase;

        public MailAPI(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _mailBase = $"{httpClient.BaseAddress}mail/v1";
        }

        public async Task<bool> SendTestEmailAsync()
        {
            try
            {
                var response = await RequestHandler.GetFromJsonAsync<string>(_httpClient, $"{_mailBase}/send/test");

                if (response == "success")
                {
                    return true;
                }
                else
                {
                    throw new HttpRequestException("Mail failed to send");
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
