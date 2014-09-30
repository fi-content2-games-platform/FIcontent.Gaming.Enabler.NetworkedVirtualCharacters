using System;
using System.Collections.Generic;
using UnityEngine;

namespace NativeClient
{
	public class WorldManager
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NativeClient.WorldManager"/> class.
		/// </summary>
		/// <param name="communicator">Connected communicator.</param>
		public WorldManager (Communicator communicator)
		{
			if (!communicator.IsConnected)
				throw new ArgumentException ("Communicator must be connected when passed to WorldManager constructor.");

			this.communicator = communicator;

			QueryClientServices ();
		}

		/// <summary>
		/// Occurs when the world has loaded initial entities.
		/// </summary>
		public event EventHandler Loaded;
		public event EventHandler<EntityCreatedEventArgs> EntityCreated;

		public void CreateEntity ()
		{
			CreateEntityAt (Vector3.zero);
		}

		public void CreateEntityAt (Vector3 position)
		{
			EntityInfo newEntity = new EntityInfo
            {
                Position = position,
                Orientation = Quaternion.identity,
                IsLocallyCreated = true
            };

			int createCall = communicator.Call ("editing.createEntityAt", newEntity.Position);
			communicator.AddReplyHandler (createCall, delegate(CallReply reply) {
				newEntity.Guid = reply.RetValue.ToString ();

				if (EntityCreated != null)
					EntityCreated (this, new EntityCreatedEventArgs (newEntity.Guid, newEntity));
			});
		}

		public void GetDragon ()
		{
			EntityInfo newEntity = new EntityInfo
			{
				Position = Vector3.zero,
				Orientation = Quaternion.identity,
				IsLocallyCreated = true
			};

			int createCall = communicator.Call ("NVC.getDragon");
			communicator.AddReplyHandler (createCall, delegate(CallReply reply) {
				newEntity.Guid = reply.RetValue.ToString ();
				
				if (EntityCreated != null)
					EntityCreated (this, new EntityCreatedEventArgs (newEntity.Guid, newEntity));
			});
		}
			
		public void UpdateBones (EntityInfo e, List<Transform> transforms)
		{
			var pos = new List<Vector3> ();
			var rot = new List<Quaternion> ();
			foreach(var t in transforms)
			{
				if (t == null) {
					pos.Add(Vector3.zero);
					rot.Add(Quaternion.identity);
					continue;
				}
				pos.Add(t.localPosition);
				rot.Add(t.localRotation);
			}

			communicator.Call ("NVC.updateBones", e.Guid,  pos, rot, Timestamps.UnixTimestamp);

			Debug.Log("updateBones " + e.Guid);
		}

		/// <summary>
		/// Returns the list of Entities known to the client. Please use locks when accessing this field.
		/// </summary>
		public List<EntityInfo> Entities = new List<EntityInfo> ();

		private void QueryClientServices ()
		{
			List<string> requiredServices = new List<string> { "objectsync", "avatar", "editing", "location" };
			int callID = communicator.Call ("kiara.implements", requiredServices);
			communicator.AddReplyHandler (callID, HandleQueryClientServicesReply);
		}

		private void HandleQueryClientServicesReply (CallReply reply)
		{
			if (!reply.Success)
				Console.WriteLine ("Failed to request client services: {0}", reply.Message);

			List<bool> retValue = reply.RetValue as List<bool>;
			if (!retValue.TrueForAll (s => s)) {
				Console.WriteLine ("Required client services are not supported");
			}

			RegisterHandlers ();
			RequestAllObjects ();
		}

		private void RegisterHandlers ()
		{
			string handleNewObject = communicator.RegisterFunc (HandleNewObject);
			communicator.Call ("objectsync.notifyAboutNewObjects", new List<int>{0}, handleNewObject);

			string handleUpdate = communicator.RegisterFunc (HandleUpdate);
			communicator.Call ("objectsync.notifyAboutObjectUpdates", new List<int> {0}, handleUpdate);
		}

		public event EventHandler<NewObjectEventArgs> ReceivedNewObject;

		private void HandleNewObject (Dictionary<string, object> entityInfo)
		{
			EntityInfo newEntity = new EntityInfo {
                Guid = entityInfo["guid"].ToString()
            };

			if (entityInfo ["location"] != null) {
				Dictionary<string, Dictionary<string, object>> locationValues = entityInfo ["location"] as Dictionary<string, Dictionary<string, object>>;
                
				if (locationValues ["location"] ["position"] != null)
					newEntity.Position = (Vector3)locationValues ["location"] ["position"];
				else
					newEntity.Position = Vector3.zero;

				if (locationValues ["location"] ["orientation"] != null)
					newEntity.Orientation = (Quaternion)locationValues ["location"] ["orientation"];
				else
					newEntity.Orientation = Quaternion.identity;
			}
            

			lock (Entities)
				Entities.Add (newEntity);

			if (ReceivedNewObject != null)
				ReceivedNewObject (this, new NewObjectEventArgs (newEntity));
		}

		private void HandleNewObject (CallRequest request)
		{
			Dictionary<string, object> entityInfo = request.Args [0] as Dictionary<string, object>;
			HandleNewObject (entityInfo);
		}

		public event EventHandler<UpdateEventArgs> ReceivedUpdates;

		private void HandleUpdate (CallRequest request)
		{
			List<UpdateInfo> receivedUpdates = request.Args [0] as List<UpdateInfo>;
			if (ReceivedUpdates != null)
				ReceivedUpdates (this, new UpdateEventArgs (receivedUpdates));
		}

		private void RequestAllObjects ()
		{
			int callID = communicator.Call ("objectsync.listObjects");
			communicator.AddReplyHandler (callID, HandleRequestAllObjectReply);
		}

		private void HandleRequestAllObjectReply (CallReply reply)
		{
			if (!reply.Success)
				Console.WriteLine ("Failed to list objects: {0}", reply.Message);

			List<Dictionary<string, object>> retValue = reply.RetValue as List<Dictionary<string, object>>;
			retValue.ForEach (HandleNewObject);

			if (Loaded != null)
				Loaded (this, new EventArgs ());
		}

		private Communicator communicator;

		//static System.Random random = new System.Random();        
	}
}

