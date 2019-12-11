using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDMSG;

namespace StationLog
{
    public class NewCommand : PubSubEvent<MsgYDCommand> { }
}
