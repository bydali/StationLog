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
using MahApps.Metro.Controls;
using Prism.Events;
using YDMSG;

namespace StationLog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private IEventAggregator eventAggregator;

        public MainWindow(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            InitializeComponent();

            RegisterALLEvent();
            IO.ReceiveMsg(eventAggregator);
        }

        private void RegisterALLEvent()
        {
            eventAggregator.GetEvent<NewCommand>().Unsubscribe(ReceiveCmd);
            eventAggregator.GetEvent<NewCommand>().Subscribe(ReceiveCmd, ThreadOption.UIThread);

            eventAggregator.GetEvent<AgentSignCommand>().Unsubscribe(AgentSignCmd);
            eventAggregator.GetEvent<AgentSignCommand>().Subscribe(AgentSignCmd, ThreadOption.UIThread);
        }

        private void AgentSignCmd(MsgSign data)
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

        /// <summary>
        /// 接收新的命令，并显示在单独的窗口中
        /// </summary>
        /// <param name="cmd"></param>
        private void ReceiveCmd(MsgDispatchCommand cmd)
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

        private void Login(object sender, RoutedEventArgs e)
        {
            LoginWindow window = new LoginWindow();
            window.ShowDialog();
        }

        private void ReportTime(object sender, RoutedEventArgs e)
        {
            ReportTimeWindow window = new ReportTimeWindow();
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

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
