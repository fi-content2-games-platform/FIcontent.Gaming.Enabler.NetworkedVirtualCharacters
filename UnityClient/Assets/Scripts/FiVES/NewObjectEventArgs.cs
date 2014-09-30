using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NativeClient
{
    public class NewObjectEventArgs : EventArgs
    {
        public NewObjectEventArgs(EntityInfo newObjectInfo)
        {
            entityInfo = newObjectInfo;
        }

        public EntityInfo EntityInfo
        {
            get { return entityInfo; }
        }

        EntityInfo entityInfo;
    }
}
