using System;
using System.Threading.Tasks;

namespace InQuant.MQ
{
    public interface IProducer : IDisposable
    {
        Task Produce<T>(string topic, T m);

        Task Produce<T>(string topic, string key, T m);
    }
}
