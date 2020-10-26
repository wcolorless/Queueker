using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Queueker.core.Protocol;

namespace Queueker.core.Client
{
    public interface IQueuekerClient
    {
        Task PublishObjectAsJson<T>(T obj, string queueName);
        Task<QueueRequestResult<T>> GetObjectFromJson<T>(string queueName);
        Task<List<QueueItemInfo>> GetInfo();
        Task PurgeQueue(string queueName);
        Task RemoveQueue(string queueName);
        Task AddNewQueue(string queueName, long limit);
        Task ChangeLoginAndPassword(string login, string password);
    }
}
