using System;
using UnityEngine;

namespace NativeClient
{
    public class EntityInfo
    {
        public string Guid;
        public Vector3 Position;
        public Quaternion Orientation;
        public bool IsLocallyCreated = false;

		#region Overrides

		public override bool Equals (object obj)
		{
			EntityInfo other = obj as EntityInfo;

			if (other == null)
				return false;
			else
				return this.Guid.Equals(other.Guid);
		}

		#endregion 


		#region Operators
						
		public static implicit operator bool (EntityInfo exists)
		{
			return exists != null;
		}

		#endregion
    }
}

