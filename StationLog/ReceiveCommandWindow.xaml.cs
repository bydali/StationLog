using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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
    /// ReceiveCommandWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ReceiveCommandWindow : MetroWindow
    {
        private ObservableCollection<MsgYDCommand> ReceivedCmds;
        public ReceiveCommandWindow(ObservableCollection<MsgYDCommand> ReceivedCmds)
        {
            this.ReceivedCmds = ReceivedCmds;
            DataContext = ReceivedCmds;

            InitializeComponent();

            if (ReceivedCmds.Count != 0)
            {
                CmdGrid.DataContext = ReceivedCmds.First();
                cmdsLb.SelectedItem = ReceivedCmds.First();
            }
        }

        private void ShowCmd(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var cmd = (MsgYDCommand)((Grid)sender).DataContext;
                CmdGrid.DataContext = cmd;
                cmdsLb.SelectedItem = cmd;
            }
        }

        public void ChangeCmd()
        {
            cmdsLb.SelectedItem = ReceivedCmds.First();
            CmdGrid.DataContext = ReceivedCmds.First();
        }

        private async void CheckCmd(object sender, RoutedEventArgs e)
        {
            if (CmdGrid.DataContext != null)
            {
                var cmd = (MsgYDCommand)CmdGrid.DataContext;

                MsgReceipt check = new MsgReceipt(cmd.CmdSN, DateTime.Now.ToString(),
                    ConfigurationManager.ConnectionStrings["Station"].ConnectionString);

                var controller = await this.ShowProgressAsync("", "签收中，正在发送确认信息");
                controller.SetIndeterminate();

                try
                {
                    await Task.Run(() =>
                                    {
                                        IO.SendMsg(check);
                                    });

                    await controller.CloseAsync();

                    var station= cmd.Targets.Where(i => i.IsSelected == true &&
                i.Name == ConfigurationManager.ConnectionStrings["Station"].ConnectionString).
                First();
                    station.IsChecked = true;
                    station.CheckTime = check.CheckTime;
                    cmd.Targets = cmd.Targets;
                }
                catch (Exception except)
                {
                    await controller.CloseAsync();
                    MessageBox.Show(except.Message, "确认失败");
                }

            }
        }
    }
}
