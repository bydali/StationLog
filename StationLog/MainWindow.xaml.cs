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
        private ObservableCollection<MsgYDCommand> ReceivedCmds;

        public MainWindow(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            this.eventAggregator = eventAggregator;
            InitialData();
            RegisterALLEvent();
            IO.ReceiveMsg(eventAggregator);
        }

        private void InitialData()
        {
            Title = ConfigurationManager.ConnectionStrings["ClientName"].ConnectionString;

            ReceivedCmds = new ObservableCollection<MsgYDCommand>();
            BindingOperations.EnableCollectionSynchronization(ReceivedCmds, new object());

        }

        private void RegisterALLEvent()
        {
            eventAggregator.GetEvent<NewCommand>().Unsubscribe(ReceiveCmd);
            eventAggregator.GetEvent<NewCommand>().Subscribe(ReceiveCmd);
        }

        private void ReceiveCmd(MsgYDCommand cmd)
        {
            ReceivedCmds.Insert(0, cmd);

            Dispatcher.InvokeAsync(new Action(() =>
            {
                if (Application.Current.Windows.OfType<ReceiveCommandWindow>().Count() == 0)
                {
                    ReceiveCommandWindow window = new ReceiveCommandWindow(ReceivedCmds);
                    window.Show();
                }
                else
                {
                    Application.Current.Windows.OfType<ReceiveCommandWindow>().First().Activate();
                    Application.Current.Windows.OfType<ReceiveCommandWindow>().First().ChangeCmd();
                }
            }));
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
                ReceiveCommandWindow window = new ReceiveCommandWindow(ReceivedCmds);
                window.Show();
            }
        }
    }
}
