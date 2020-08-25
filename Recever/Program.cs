using System;

//A
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Threading;
using System.Text;
using System.Linq;

namespace Recever
{
    class Program
    {
        const string HOST_NAME = "127.0.0.1";
        const string QUEUE_NAME = "queue_1";

        const bool DURABLE_VALUE = false;
        const bool EXCLUSIVE_VALUE = false;
        const bool AUTODELETE_VALUE = false;


        static void Main(string[] args)
        {
            Console.WriteLine("Receiver Start");

            //1.factory
            var factory = new ConnectionFactory() { HostName = HOST_NAME };

            //2.connection
            using (var connection = factory.CreateConnection())
            {
                //3.channel
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: QUEUE_NAME, durable: DURABLE_VALUE, exclusive: EXCLUSIVE_VALUE, autoDelete: AUTODELETE_VALUE, arguments: null);
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    Console.WriteLine("Waiting for message...");

                    //4.consumer
                    var consumer = new EventingBasicConsumer(channel);

                    //5.receive handler
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var q = JsonConvert.DeserializeObject<Que>(Encoding.UTF8.GetString(body.ToArray()));
                        Console.WriteLine("Received '{0}'", JsonConvert.SerializeObject(q));

                        //랜덤하게 작업 지연시키기
                        Random r = new Random();
                        int i = r.Next(10, 3000);
                        Thread.Sleep(i);

                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };

                    channel.BasicConsume(queue: QUEUE_NAME, autoAck: false, consumer: consumer);
                    Console.WriteLine("Press  [enter] to exit.");
                    Console.ReadLine();
                }
            }



        }
    }
}
