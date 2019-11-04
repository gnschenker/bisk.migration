using System.Threading.Tasks;

namespace Bisk.Migration
{
    public interface IMessageHandler<TKey, TMessage> where TMessage: class
    {
        Task Handles(TKey key, TMessage message);
    }
}