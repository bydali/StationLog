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
            ConnectionFactory factory = new ConnectionFactory { HostName = "39.108.177.237", UserName = "admin", Password = "admin", VirtualHost = "/" };
            using (IConnection conn = factory.CreateConnection())
            {
                using (IModel im = conn.CreateModel())
                {
                    while (true)
                    {
                        BasicGetResult res = im.BasicGet("rabbitmq_query", true);
                        if (res != null)
                        {
                            Console.WriteLine("receiver:" + Encoding.UTF8.GetString(res.Body));
                        }
                    }
                }
            }
        }
    }
}
