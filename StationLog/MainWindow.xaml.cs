using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DSIM.Communications;
using MahApps.Metro.Controls;
using Prism.Events;

namespace StationLog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private IEventAggregator eventAggregator;

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        public MainWindow(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            InitializeComponent();

            RegisterLocalEvent();
            RegisterMQIO();

            IO.ReceiveMsg(eventAggregator);
        }

        private void RegisterLocalEvent()
        {
            eventAggregator.GetEvent<EditReportMsg>().Unsubscribe(LocalNewLog);
            eventAggregator.GetEvent<EditReportMsg>().Subscribe(LocalNewLog, ThreadOption.UIThread);
        }

        /// <summary>
        /// 编辑完某条报点消息后，更新界面
        /// </summary>
        /// <param name="msg"></param>
        private void LocalNewLog(MsgTrainTimeReport msg)
        {
            var appVM = (AppVM)DataContext;
            logDG.DataContext = null;
            logDG.DataContext = appVM.TimeTable;
        }

        private void RegisterMQIO()
        {
            eventAggregator.GetEvent<NewCommand>().Unsubscribe(ReceiveCmd);
            eventAggregator.GetEvent<NewCommand>().Subscribe(ReceiveCmd, ThreadOption.UIThread);

            eventAggregator.GetEvent<AgentSignCommand>().Unsubscribe(AgentSignCmd);
            eventAggregator.GetEvent<AgentSignCommand>().Subscribe(AgentSignCmd, ThreadOption.UIThread);

            eventAggregator.GetEvent<NewReportNet>().Unsubscribe(NewReportLog);
            eventAggregator.GetEvent<NewReportNet>().Subscribe(NewReportLog, ThreadOption.UIThread);

        }

        /// <summary>
        /// 接收调度命令，并显示签收窗口
        /// </summary>
        /// <param name="cmd"></param>
        private void ReceiveCmd(MsgDispatchCommand cmd)
        {
            var targets = cmd.Targets.Where(i => i.IsSelected == true &&
                                        i.Name == ConfigurationManager.ConnectionStrings["ClientName"].ConnectionString);
            if (targets.Count() != 0)
            {
                var appVM = (AppVM)DataContext;
                var receivedLst = appVM.ReceivedCmds;
                receivedLst.Insert(0, cmd);

                if (Application.Current.Windows.OfType<ReceiveCommandWindow>().Count() == 0)
                {
                    ReceiveCommandWindow window = new ReceiveCommandWindow(receivedLst);
                    window.Show();
                }
                else
                {
                    var window = Application.Current.Windows.OfType<ReceiveCommandWindow>().First();
                    window.WindowState = WindowState.Normal;
                    Application.Current.Windows.OfType<ReceiveCommandWindow>().First().ChangeCmd(cmd);
                }
            }
        }

        /// <summary>
        /// 接收代签命令，并显示签收窗口
        /// </summary>
        /// <param name="data"></param>
        private void AgentSignCmd(MsgCommandSign data)
        {
            if (data.AgentTarget == ConfigurationManager.ConnectionStrings["ClientName"].ConnectionString)
            {
                var appVM = (AppVM)DataContext;
                var cmd = appVM.ReceivedCmds.Where(i => i.CmdSN == data.CmdSN).First();

                cmd.OneTargetSigned(data);

                if (Application.Current.Windows.OfType<ReceiveCommandWindow>().Count() == 0)
                {
                    ReceiveCommandWindow window = new ReceiveCommandWindow(appVM.ReceivedCmds);
                    window.Show();
                    window.ChangeCmd(cmd);
                }
                else
                {
                    var window = Application.Current.Windows.OfType<ReceiveCommandWindow>().First();
                    window.WindowState = WindowState.Normal;
                    Application.Current.Windows.OfType<ReceiveCommandWindow>().First().ChangeCmd(cmd);
                }
            }
        }

        /// <summary>
        /// 接收报点信息
        /// </summary>
        /// <param name="msg"></param>
        private void NewReportLog(MsgTrainTimeReport msg)
        {
            var appVM = (AppVM)DataContext;

            var result = appVM.TimeTable.Where(i => i.Train == msg.Train);
            if (result.Count()!=0)
            {
                var idx = appVM.TimeTable.IndexOf(result.First());
                appVM.TimeTable.Remove(result.First());
                appVM.TimeTable.Insert(idx, msg);
            }
            else
            {
                appVM.TimeTable.Add(msg);
            }
        }

        #region 界面按钮事件

        private void Login(object sender, RoutedEventArgs e)
        {
            LoginWindow window = new LoginWindow();
            window.ShowDialog();
        }

        private void ReportTime(object sender, RoutedEventArgs e)
        {
            EditTimeWindow window = new EditTimeWindow();
            window.ShowDialog();
        }

        private void HangingTrain(object sender, RoutedEventArgs e)
        {
            HangingWindow window = new HangingWindow();
            window.ShowDialog();
        }

        private void CurrentTrain(object sender, RoutedEventArgs e)
        {
            CurrentTrainWindow window = new CurrentTrainWindow();
            window.ShowDialog();
        }

        private void QueryTask(object sender, RoutedEventArgs e)
        {
            QueryTaskWindow window = new QueryTaskWindow();
            window.ShowDialog();
        }

        private void QueryCommand(object sender, RoutedEventArgs e)
        {
            QueryCommandWindow window = new QueryCommandWindow();
            window.ShowDialog();
        }

        private void ParameterSetting(object sender, RoutedEventArgs e)
        {
            ParameterSettingWindow window = new ParameterSettingWindow();
            window.ShowDialog();
        }

        /// <summary>
        /// 打开签收命令列表窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceiveCommand(object sender, RoutedEventArgs e)
        {
            if (Application.Current.Windows.OfType<ReceiveCommandWindow>().Count() == 0)
            {
                var appVM = (AppVM)DataContext;
                var receivedLst = appVM.ReceivedCmds;
                ReceiveCommandWindow window = new ReceiveCommandWindow(receivedLst);
                window.Show();
            }
        }

        /// <summary>
        /// 右键打开某条报点消息的编辑界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditTrainTime(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                var msg = (MsgTrainTimeReport)((DataGridRow)sender).DataContext;
                EditTimeWindow window = new EditTimeWindow(eventAggregator, msg);
                window.Show();

            }
        }


        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var msg = new MsgTrainTimeReport();
            msg.Category = MsgCategoryEnum.TrainTimeReport;
            IO.SendMsg(msg, "DSIM.TrainTime.Report");
        }
    }
}
