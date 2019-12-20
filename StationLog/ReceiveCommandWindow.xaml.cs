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
        private ObservableCollection<MsgDispatchCommand> ReceivedCmds;

        public ReceiveCommandWindow(ObservableCollection<MsgDispatchCommand> ReceivedCmds)
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

        /// <summary>
        /// 双击查看命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowCmd(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var cmd = (MsgDispatchCommand)((Grid)sender).DataContext;
                CmdGrid.DataContext = cmd;
                cmdsLb.SelectedItem = cmd;
            }
        }

        public void ChangeCmd(MsgDispatchCommand cmd)
        {
            cmdsLb.SelectedItem = cmd;
            CmdGrid.DataContext = cmd;
        }

        private async void CheckCmd(object sender, RoutedEventArgs e)
        {
            if (CmdGrid.DataContext != null)
            {
                var cmd = (MsgDispatchCommand)CmdGrid.DataContext;

                MsgSign check = new MsgSign(cmd.CmdSN, DateTime.Now.ToString(),
                    ConfigurationManager.ConnectionStrings["ClientName"].ConnectionString,
                    ConfigurationManager.ConnectionStrings["User"].ConnectionString);

                try
                {
                    await Task.Run(() =>
                                    {
                                        IO.SendMsg(check, "DSIM.Command.Sign");
                                    });
                    cmd.OneTargetSigned(check);
                }
                catch (Exception except)
                {
                    MessageBox.Show(except.Message);
                }

            }
        }
    }
}
