using DSIM.Communications;
using MahApps.Metro.Controls;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YDMSG;

namespace StationLog
{
    /// <summary>
    /// ReportTimeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditTimeWindow : MetroWindow
    {
        private IEventAggregator eventAggregator;

        private MsgTrainTimeReport msg;

        public EditTimeWindow(IEventAggregator eventAggregator, MsgTrainTimeReport msg)
        {
            this.eventAggregator = eventAggregator;
            this.msg = msg;
            InitializeComponent();

            SetData(msg);
        }

        private void SetData(MsgTrainTimeReport msg)
        {
            if (msg.Action=="到达")
            {
                arriveTrainTB.Text = msg.Train;
                arriveChannelTB.Text = msg.Channel;
                arriveNearbyTB.Text = msg.NearbyStation;
                arriveNearbyTimeTB.SelectedTime = msg.NearbyTime;
                arrivePlanTB.SelectedTime = msg.PlanTime;
                arriveActualTB.SelectedTime = msg.ActualTime;
            }
            if (msg.Action == "出发")
            {
                departTrainTB.Text = msg.Train;
                departChannelTB.Text = msg.Channel;
                departNearbyTB.Text = msg.NearbyStation;
                departNearbyTimeTB.SelectedTime = msg.NearbyTime;
                departPlanTB.SelectedTime = msg.PlanTime;
                departActualTB.SelectedTime = msg.ActualTime;
            }
        }

        public EditTimeWindow()
        {
            InitializeComponent();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (msg!=null)
            {
                if (msg.Action == "到达")
                {
                    msg.Train = arriveTrainTB.Text;
                    msg.Channel = arriveChannelTB.Text;
                    msg.NearbyStation = arriveNearbyTB.Text;
                    msg.NearbyTime = arriveNearbyTimeTB.SelectedTime.GetValueOrDefault();
                    msg.PlanTime = arrivePlanTB.SelectedTime.GetValueOrDefault();
                    msg.ActualTime = arriveActualTB.SelectedTime.GetValueOrDefault();
                }
                if (msg.Action == "出发")
                {
                    msg.Train = departTrainTB.Text;
                    msg.Channel = departChannelTB.Text;
                    msg.NearbyStation = departNearbyTB.Text;
                    msg.NearbyTime = departNearbyTimeTB.SelectedTime.GetValueOrDefault();
                    msg.PlanTime = departPlanTB.SelectedTime.GetValueOrDefault();
                    msg.ActualTime = departActualTB.SelectedTime.GetValueOrDefault();
                }

                eventAggregator.GetEvent<EditReportMsg>().Publish(msg);
            }
        }
    }
}
