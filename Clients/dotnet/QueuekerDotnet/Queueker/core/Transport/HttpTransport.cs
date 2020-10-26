using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Queueker.core.Protocol;
using Queueker.core.Settings;

namespace Queueker.core.Transport
{
    public class HttpTransport : ITransport
    {
        private HttpClient _httpClient;
        private QueuekerSettings _queuekerSettings;
        public HttpTransport(QueuekerSettings queuekerSettings)
        {
            _queuekerSettings = queuekerSettings;
            _httpClient = new HttpClient();
        }

        public async Task<string> SendToServer(MessageInfo messageInfo)
        {
            try
            {
                var json = JsonConvert.SerializeObject(messageInfo);
                var data = Encoding.UTF8.GetBytes(json);
                var content = new ByteArrayContent(data);
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                var response = await _httpClient.PostAsync(new Uri($"{_queuekerSettings.Host}"), content);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return result;
                }

                return string.Empty;
            }
            catch (Exception e)
            {
                throw new Exception($"HttpTransport SendToServer Error: {e.Message}");
            }
        }
    }
}
