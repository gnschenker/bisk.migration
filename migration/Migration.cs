using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Bisk.Messages;
using Bisk.Messaging;

namespace Bisk.Migration
{
    public class Migration
    {
        private static readonly AutoResetEvent _closing = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            Task.Factory.StartNew(() =>
            {
                Consuming();
            });
            Console.WriteLine("Waiting for messages...");
            Console.WriteLine(" Press CTRL-c to exit.");
            Console.CancelKeyPress += (sender, _) => {
                Console.WriteLine("Exiting...");
                _closing.Set();
            }; 
            _closing.WaitOne();
        }
        
        private static void Consuming()
        {
            Console.WriteLine(">>> Start consuming!");
            var consumer = GetConsumer();
            consumer.Subscribe<string, DisciplineSelected>("migration-manager", async (key, message) =>
            {
                await new DisciplineSelectedHandler().Handles(key, message);
            });

            Console.WriteLine("Marking microservice as ready!");
            using(File.Create("ready")){ }

            _closing.WaitOne();
            consumer.Dispose();
            Console.WriteLine("<<< End consuming!");
        }

        private static IConsumerStrategy GetConsumer()
        {
            return new RabbitMqConsumerStrategy();
        }
    }
}
