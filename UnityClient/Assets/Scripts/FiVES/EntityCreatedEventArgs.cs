using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NativeClient
{
    public class EntityCreatedEventArgs : EventArgs
    {
        public EntityCreatedEventArgs(string guid, EntityInfo info)
        {
            entityGuid = guid;
            entityInfo = info;
        }

        public EntityInfo EntityInfo
        {
            get { return entityInfo; }
        }

        public string EntityGuid
        {
            get { return entityGuid; }
        }
        EntityInfo entityInfo;
        string entityGuid;
    }
}
