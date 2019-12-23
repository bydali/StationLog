using DSIM.Communications;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace StationLog
{
    class AppVM : INotifyPropertyChanged
    {
        private string user;
        private IEventAggregator eventAggregator;
        private string appTitle;

        public string AppTitle
        {
            get => appTitle; set
            {
                appTitle = value;
                OnProPertyChanged(new PropertyChangedEventArgs("AppTitle"));
            }
        }
        public ObservableCollection<MsgDispatchCommand> ReceivedCmds { get; set; }

        private ObservableCollection<MsgTrainTimeReport> timeTable;
        public ObservableCollection<MsgTrainTimeReport> TimeTable
        {
            get => timeTable;
            set
            {
                timeTable = value;
                OnProPertyChanged(new PropertyChangedEventArgs("TimeTable"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnProPertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        public AppVM(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            user = ConfigurationManager.ConnectionStrings["User"].ConnectionString;
            AppTitle = ConfigurationManager.ConnectionStrings["ClientName"].ConnectionString;
            AppTitle = AppTitle + "\t" + "用户：" + user;

            ReceivedCmds = new ObservableCollection<MsgDispatchCommand>();
            BindingOperations.EnableCollectionSynchronization(ReceivedCmds, new object());

            TimeTable = new ObservableCollection<MsgTrainTimeReport>();
            GenerateTimeTable(TimeTable);
        }

        private void GenerateTimeTable(ObservableCollection<MsgTrainTimeReport> timeTable)
        {
            for (int i = 0; i < 10; i++)
            {
                TimeTable.Add(new MsgTrainTimeReport()
                {
                    Train = "测试" + i.ToString()
                });
            }
        }
    }

}
