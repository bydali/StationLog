using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using YDMSG;

namespace StationLog
{
    class AppVM : BindableBase
    {
        private IEventAggregator eventAggregator;
        private string appTitle;
        public string AppTitle { get => appTitle; set { SetProperty(ref appTitle, value); } }
        public ObservableCollection<MsgDispatchCommand> ReceivedCmds { get; set; }

        public ObservableCollection<TrainTask> TimeTable { get; set; }

        public AppVM(IEventAggregator eventAggregator)
        {
            AppTitle = ConfigurationManager.ConnectionStrings["ClientName"].ConnectionString;

            ReceivedCmds = new ObservableCollection<MsgDispatchCommand>();
            BindingOperations.EnableCollectionSynchronization(ReceivedCmds, new object());

            TimeTable = new ObservableCollection<TrainTask>();
            TimeTable.Add(new TrainTask() { TrainNum = "G1002" });
        }
    }

    class TrainTask
    {
        public string TrainNum { get; set; }
        public string NeighborDepartTime{ get; set; }
        public string PlanArriveTime { get; set; }
        public string ArriveTime { get; set; }
        public string PlanDepartTime { get; set; }
        public string DepartTime { get; set; }

        public TrainTask()
        {

        }
    }
}
