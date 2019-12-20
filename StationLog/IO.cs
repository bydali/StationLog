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
using System.Windows;
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

            // 以下为测试代码
            try
            {
                ConnectionFactory factory = new ConnectionFactory { HostName = "39.108.177.237", Port = 5672, UserName = "admin", Password = "admin" };
                using (IConnection conn = factory.CreateConnection())
                {
                    using (IModel im = conn.CreateModel())
                    {
                        List<string> allTopic = new List<string>()
                        { "DSIM.Command.Transmit",
                        "DSIM.Command.AgentSign",
                        };

                        var queueName = ConfigurationManager.ConnectionStrings["User"].ConnectionString;

                        im.ExchangeDeclare("amq.topic", ExchangeType.Topic, durable: true);
                        im.QueueDeclare(queueName);
                        foreach (var item in allTopic)
                        {
                            im.QueueBind(queueName, "amq.topic", item);
                        }

                        await Task.Run(() =>
                        {
                            while (true)
                            {
                                BasicGetResult res = im.BasicGet(queueName, true);
                                if (res != null)
                                {
                                    var json = Encoding.UTF8.GetString(res.Body);

                                    var split = json.LastIndexOf("/");
                                    var suffix = json.Substring(split + 1);
                                    var content = json.Substring(0, split);

                                    switch (suffix)
                                    {
                                        case ("DSIM.Command.Transmit"):
                                            var data = JsonConvert.DeserializeObject<MsgDispatchCommand>(content);

                                            var targets = data.Targets.Where(i => i.IsSelected == true &&
                                        i.Name == ConfigurationManager.ConnectionStrings["ClientName"].ConnectionString);
                                            if (targets.Count() != 0)
                                            {
                                                eventAggregator.GetEvent<NewCommand>().Publish(data);
                                            }
                                            break;
                                        case ("DSIM.Command.AgentSign"):
                                            var data1 = JsonConvert.DeserializeObject<MsgSign>(content);
                                            eventAggregator.GetEvent<AgentSignCommand>().Publish(data1);
                                            break;
                                        default:
                                            break;
                                    }

                                }
                            }
                        });
                    }
                }
            }
            catch (Exception except)
            {
                MessageBox.Show(except.Message);
            }
        }

        public static void SendMsg(object msg, string topic)
        {
            // 以下为测试代码
            ConnectionFactory factory = new ConnectionFactory { HostName = "39.108.177.237", Port = 5672, UserName = "admin", Password = "admin" };
            using (IConnection conn = factory.CreateConnection())
            {
                using (IModel im = conn.CreateModel())
                {
                    string json = JsonConvert.SerializeObject(msg);
                    json += "/" + topic;
                    byte[] message = Encoding.UTF8.GetBytes(json);

                    im.ExchangeDeclare("amq.topic", ExchangeType.Topic, durable: true);
                    im.BasicPublish("amq.topic", topic, null, message);
                }
            }
        }

        private static void RabbitMQ_MessageArrived(object sender, MsgCategoryEnum e)
        {
            eventAggregator.GetEvent<PubSubEvent<string>>().Publish("asdf");
        }
    }
}
