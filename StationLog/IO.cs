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

namespace StationLog
{
    class IO
    {
        private static IEventAggregator eventAggregator;
        private static MQHelper _mqHelper;

        public static async void ReceiveMsg(IEventAggregator aggregator)
        {
            eventAggregator = aggregator;

            MQHelper.ConnectionString = ConfigurationManager.ConnectionStrings["RabbitMQ"].ConnectionString;
            _mqHelper = new MQHelper
            {
                ClientSubscriptionId = ConfigurationManager.ConnectionStrings["ClientName"].ConnectionString + "-" +
                ConfigurationManager.ConnectionStrings["User"].ConnectionString
            };
            _mqHelper.Topics.Add("DSIM.Command.Transmit");
            _mqHelper.Topics.Add("DSIM.Command.AgentSign");
            _mqHelper.Topics.Add("DSIM.TrainTime.Report");
            _mqHelper.Subcribe();
            _mqHelper.MessageArrived += RabbitMQ_MessageArrived;

            // 以下为测试代码
            //try
            //{
            //    ConnectionFactory factory = new ConnectionFactory { HostName = "39.108.177.237", Port = 5672, UserName = "admin", Password = "admin" };
            //    using (IConnection conn = factory.CreateConnection())
            //    {
            //        using (IModel im = conn.CreateModel())
            //        {
            //            List<string> allTopic = new List<string>()
            //            { "DSIM.Command.Transmit",
            //            "DSIM.Command.AgentSign",
            //            "DSIM.TrainTime.Report",
            //            };

            //            var queueName = ConfigurationManager.ConnectionStrings["User"].ConnectionString;

            //            im.ExchangeDeclare("amq.topic", ExchangeType.Topic, durable: true);
            //            im.QueueDeclare(queueName);
            //            foreach (var item in allTopic)
            //            {
            //                im.QueueBind(queueName, "amq.topic", item);
            //            }

            //            await Task.Run(() =>
            //            {
            //                while (true)
            //                {
            //                    BasicGetResult res = im.BasicGet(queueName, true);
            //                    if (res != null)
            //                    {
            //                        var json = Encoding.UTF8.GetString(res.Body);

            //                        var split = json.LastIndexOf("/");
            //                        var suffix = json.Substring(split + 1);
            //                        var content = json.Substring(0, split);

            //                        switch (suffix)
            //                        {
            //                            case ("DSIM.Command.Transmit"):
            //                                var data = JsonConvert.DeserializeObject<MsgDispatchCommand>(content);
            //                                eventAggregator.GetEvent<NewCommand>().Publish(data);
            //                                break;
            //                            case ("DSIM.Command.AgentSign"):
            //                                var data1 = JsonConvert.DeserializeObject<MsgCommandSign>(content);
            //                                eventAggregator.GetEvent<AgentSignCommand>().Publish(data1);
            //                                break;
            //                            case ("DSIM.Command.Report"):
            //                                var data2 = JsonConvert.DeserializeObject<MsgTrainTimeReport>(content);
            //                                eventAggregator.GetEvent<NewReportNet>().Publish(data2);
            //                                break;
            //                            default:
            //                                break;
            //                        }

            //                    }
            //                }
            //            });
            //        }
            //    }
            //}
            //catch (Exception except)
            //{
            //    MessageBox.Show(except.Message);
            //}
        }

        private static void RabbitMQ_MessageArrived(object sender, MsgCategoryEnum e)
        {
            switch (e)
            {
                case (MsgCategoryEnum.CommandTransmit):
                    eventAggregator.GetEvent<NewCommand>().Publish((MsgDispatchCommand)sender);
                    break;
                case (MsgCategoryEnum.CommandAgentSign):
                    eventAggregator.GetEvent<AgentSignCommand>().Publish((MsgCommandSign)sender);
                    break;
                case (MsgCategoryEnum.TrainTimeReport):
                    eventAggregator.GetEvent<NewReportNet>().Publish((MsgTrainTimeReport)sender);
                    break;
                default:
                    break;
            }

        }

        public static void SendMsg(object msg, string topic)
        {
            switch (msg.GetType().Name)
            {
                case ("MsgCommandSign"):
                    ((MsgCommandSign)msg).Topic = topic;
                    _mqHelper.Publish((MsgCommandSign)msg);
                    break;
                default:
                    break;
            }

            // 以下为测试代码
            //ConnectionFactory factory = new ConnectionFactory { HostName = "39.108.177.237", Port = 5672, UserName = "admin", Password = "admin" };
            //using (IConnection conn = factory.CreateConnection())
            //{
            //    using (IModel im = conn.CreateModel())
            //    {
            //        string json = JsonConvert.SerializeObject(msg);
            //        json += "/" + topic;
            //        byte[] message = Encoding.UTF8.GetBytes(json);

            //        im.ExchangeDeclare("amq.topic", ExchangeType.Topic, durable: true);
            //        im.BasicPublish("amq.topic", topic, null, message);
            //    }
            //}
        }
    }
}
