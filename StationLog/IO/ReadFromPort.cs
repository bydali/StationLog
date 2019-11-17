using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationLog.IO
{
    public static class ReadFromPort
    {
        public static void ReceiveMsg()
        {
            //while (true) {
            //    var factory = new ConnectionFactory() { HostName = "39.108.177.237" };
            //    using (var connection = factory.CreateConnection())
            //    using (var channel = connection.CreateModel())
            //    {
            //        channel.ExchangeDeclare(exchange: "", type: "topic");
            //        var queueName = channel.QueueDeclare().QueueName;

            //        channel.QueueBind(queue: queueName,
            //                          exchange: "",
            //                          routingKey: "anonymous.info");

            //        var consumer = new EventingBasicConsumer(channel);
            //        consumer.Received += (model, ea) =>
            //        {
            //            var body = ea.Body;
            //            var message = Encoding.UTF8.GetString(body);
            //            var routingKey = ea.RoutingKey;
            //            Console.WriteLine("Received '{0}':'{1}'",
            //                              routingKey,
            //                              message);
            //        };
            //        //channel.BasicConsume(queue: queueName,
            //        //                     autoAck: true,
            //        //                     consumer: consumer);
            //    }
            //}
        }
    }
}
