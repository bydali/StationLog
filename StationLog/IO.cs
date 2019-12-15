using DSIM.Communications;
using Newtonsoft.Json;
using Prism.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDMSG;

namespace StationLog
{
    class IO
    {
        private static IEventAggregator eventAggregator;
        private static MQHelper _mqHelper;

        public static async void ReceiveMsg(IEventAggregator aggregator)
        {
            eventAggregator = aggregator;
            //MQHelper.ConnectionString = ConfigurationManager.ConnectionStrings["RabbitMQ"].ConnectionString;
            //_mqHelper = new MQHelper
            //{
            //    ClientSubscriptionId = ConfigurationManager.ConnectionStrings["ClientID"].ConnectionString
            //};
            //_mqHelper.MessageArrived += RabbitMQ_MessageArrived;

            // 以下为测试代码
            ConnectionFactory factory = new ConnectionFactory { HostName = "39.108.177.237", Port = 5672, UserName = "admin", Password = "admin" };
            using (IConnection conn = factory.CreateConnection())
            {
                using (IModel im = conn.CreateModel())
                {
                    im.ExchangeDeclare("amq.topic", ExchangeType.Topic, durable: true);
                    im.QueueDeclare("station");
                    im.QueueBind("station", "amq.topic", "调度命令");

                    await Task.Run(() =>
                    {
                        while (true)
                        {
                            BasicGetResult res = im.BasicGet("station", true);
                            if (res != null)
                            {
                                var json = Encoding.UTF8.GetString(res.Body);

                                var cmd = JsonConvert.DeserializeObject<MsgYDCommand>(json);

                                var targets = cmd.Targets.Where(i => i.IsSelected == true &&
                                    i.Name == ConfigurationManager.ConnectionStrings["Station"].ConnectionString);
                                if (targets.Count() != 0)
                                {
                                    eventAggregator.GetEvent<NewCommand>().
                                                                        Publish(cmd);
                                }
                            }
                        }
                    });
                }
            }
        }

        public static void SendMsg(object msg)
        {
            // 以下为测试代码
            ConnectionFactory factory = new ConnectionFactory { HostName = "39.108.177.237", Port = 5672, UserName = "admin", Password = "admin" };
            using (IConnection conn = factory.CreateConnection())
            {
                using (IModel im = conn.CreateModel())
                {
                    im.ExchangeDeclare("amq.topic", ExchangeType.Topic, durable: true);

                    string json = JsonConvert.SerializeObject(msg);

                    byte[] message = Encoding.UTF8.GetBytes(json);
                    im.BasicPublish("amq.topic", "回执信息", null, message);
                }
            }
        }

        private static void RabbitMQ_MessageArrived(object sender, MsgCategoryEnum e)
        {
            eventAggregator.GetEvent<PubSubEvent<string>>().Publish("asdf");
        }
    }
}
