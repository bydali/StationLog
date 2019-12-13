using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class ReceiveCommandWindow : Window
    {
        private ObservableCollection<MsgYDCommand> ReceivedCmds;
        public ReceiveCommandWindow(ObservableCollection<MsgYDCommand> ReceivedCmds)
        {
            this.ReceivedCmds = ReceivedCmds;
            DataContext = ReceivedCmds;

            InitializeComponent();

            CmdGrid.DataContext = ReceivedCmds.First();
            cmdsLb.SelectedItem = ReceivedCmds.First();
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
    }
}
