using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Queueker.core.Protocol;
using Queueker.core.Settings;
using Queueker.core.Transport;

namespace Queueker.core.Client
{
    public class QueuekerClient : IQueuekerClient
    {
        private ITransport _transport;
        private QueuekerSettings _queuekerSettings;
        private QueuekerClient(TransportTypes transportType, QueuekerSettings settings)
        {
            _queuekerSettings = settings;
            _transport = TransportFactory.GetTransport(transportType, settings);
        }

        public static QueuekerClient Create(TransportTypes transportType, QueuekerSettings settings)
        {
            return new QueuekerClient(transportType, settings);
        }

        public async Task PublishObjectAsJson<T>(T obj, string queueName)
        {
            try
            {
                var json = JsonConvert.SerializeObject(obj);
                var messageData = Encoding.UTF8.GetBytes(json).ToList();
                var info = new MessageInfo()
                {
                    login = _queuekerSettings.Login,
                    pass = _queuekerSettings.Login,
                    quename = queueName,
                    direct = "in",
                    command = "write",
                    data = messageData
                };
                await _transport.SendToServer(info);
            }
            catch (Exception e)
            {
                Console.WriteLine($"QueuekerClient PublishObjectAsJson Error: {e.Message}");
            }
        }

        public async Task<QueueRequestResult<T>> GetObjectFromJson<T>(string queueName)
        {
            var result = new QueueRequestResult<T>() { Complete = false };
            try
            {
                var info = new MessageInfo()
                {
                    login = _queuekerSettings.Login,
                    pass = _queuekerSettings.Login,
                    quename = queueName,
                    direct = "out",
                    command = "read",
                    data = new List<byte>()
                };
                var answer = await _transport.SendToServer(info);
                var protocolAnswer = JsonConvert.DeserializeObject<ResultMessage>(answer);
                if (protocolAnswer.Result == "data")
                {
                    var data = protocolAnswer.Data;
                    var json = Encoding.UTF8.GetString(data.ToArray(), 0, data.Count);
                    result.Complete = true;
                    result.Result = JsonConvert.DeserializeObject<T>(json);
                }

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"QueuekerClient GetObjectFromJson Error: {e.Message}");
            }
            return result;
        }

        public async Task<List<QueueItemInfo>> GetInfo()
        {
            var result = new List<QueueItemInfo>();
            try
            {
                var info = new MessageInfo()
                {
                    login = _queuekerSettings.Login,
                    pass = _queuekerSettings.Login,
                    quename = "",
                    direct = "-",
                    command = "getques",
                    data = new List<byte>()
                };
                var answer = await _transport.SendToServer(info);
                var protocolAnswer = JsonConvert.DeserializeObject<ResultMessage>(answer);
                if (protocolAnswer.Result == "ok")
                {
                    var message = protocolAnswer.Message;
                    result.AddRange(JsonConvert.DeserializeObject<List<QueueItemInfo>>(message));
                }

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"QueuekerClient GetInfo Error: {e.Message}");
            }
            return result;
        }

        public async Task PurgeQueue(string queueName)
        {
            try
            {
                var info = new MessageInfo()
                {
                    login = _queuekerSettings.Login,
                    pass = _queuekerSettings.Login,
                    quename = queueName,
                    direct = "-",
                    command = "purge",
                    data = new List<byte>()
                };
                var answer = await _transport.SendToServer(info);
            }
            catch (Exception e)
            {
                Console.WriteLine($"QueuekerClient PurgeQueue Error: {e.Message}");
            }
        }

        public async Task RemoveQueue(string queueName)
        {
            try
            {
                var info = new MessageInfo()
                {
                    login = _queuekerSettings.Login,
                    pass = _queuekerSettings.Login,
                    quename = queueName,
                    direct = "-",
                    command = "removeque",
                    data = new List<byte>()
                };
                var answer = await _transport.SendToServer(info);
            }
            catch (Exception e)
            {
                Console.WriteLine($"QueuekerClient RemoveQueue Error: {e.Message}");
            }
        }

        public async Task AddNewQueue(string queueName, long limit)
        {
            try
            {
                var info = new MessageInfo()
                {
                    login = _queuekerSettings.Login,
                    pass = _queuekerSettings.Login,
                    quename = $"{queueName}:{limit}",
                    direct = "-",
                    command = "addque",
                    data = new List<byte>()
                };
                var answer = await _transport.SendToServer(info);
            }
            catch (Exception e)
            {
                Console.WriteLine($"QueuekerClient AddNewQueue Error: {e.Message}");
            }
        }

        public async Task ChangeLoginAndPassword(string login, string password)
        {
            try
            {
                var subcommand = new ProtocolInnerMessage()
                {
                    subcommand = "chpass",
                    message = $"{login}:{password}"
                };
                var info = new MessageInfo()
                {
                    login = _queuekerSettings.Login,
                    pass = _queuekerSettings.Login,
                    quename = "",
                    direct = "-",
                    command = JsonConvert.SerializeObject(subcommand),
                    data = new List<byte>()
                };
                var answer = await _transport.SendToServer(info);   
            }
            catch (Exception e)
            {
                Console.WriteLine($"QueuekerClient ChangeLoginAndPassword Error: {e.Message}");
            }
        }
    }
}
