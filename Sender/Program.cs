using System;

//A
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Threading;
using System.Text;

namespace Sender
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
            Console.WriteLine("Sender start");

            //1.factory
            var factory = new ConnectionFactory() { HostName = HOST_NAME };

            //2.connection
            using (var connection = factory.CreateConnection())
            {
                //3.channel
                using (var channel = connection.CreateModel())
                {
                    //durable   : 내구성
                    //exclusive : 배타적
                    //autoDelete: 자동삭제
                    channel.QueueDeclare(queue: QUEUE_NAME, durable: DURABLE_VALUE, exclusive: EXCLUSIVE_VALUE, autoDelete: AUTODELETE_VALUE, arguments: null);

                    int i = 0;
                    while(true)
                    {
                        //4.message body
                        Que q = new Que() { Idx = ++i, Message = string.Format("Message {0}", i) };
                        var message = JsonConvert.SerializeObject(q);
                        var body = Encoding.UTF8.GetBytes(message);

                        //5.send
                        var property = channel.CreateBasicProperties();
                        //property.Persistent = true;     //지속성
                        property.Persistent = true;     //지속성
                        channel.BasicPublish(exchange: "", routingKey: QUEUE_NAME, basicProperties: property, body: body);

                        Console.WriteLine("Send '{0}'", JsonConvert.SerializeObject(q));
                        Thread.Sleep(200); 
                    }

                }
            }
        }
    }
}
