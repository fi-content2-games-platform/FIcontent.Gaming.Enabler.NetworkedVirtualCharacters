using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NativeClient
{
    public class UpdateEventArgs : EventArgs
    {
        public UpdateEventArgs(List<UpdateInfo> updates)
        {
            receivedUpdates = updates;
        }

        public List<UpdateInfo> ReceivedUpdates
        {
            get { return receivedUpdates; }
        }

        List<UpdateInfo> receivedUpdates;
    }
}
