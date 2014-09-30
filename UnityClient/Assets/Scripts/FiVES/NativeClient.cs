using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NativeClient
{
	public class NativeClientApplication : INativeClient
	{
		public List<EntityInfo> World {
			get { return worldManager.Entities; }
		}
			
		public NativeClientApplication (string connectionString)
		{
			clientDriver = new ClientDriver ();
			clientDriver.communicator.Connected += new EventHandler (HandleConnected);
			clientDriver.communicator.OpenConnection (connectionString);	
		}

		private void HandleConnected (object sender, EventArgs e)
		{
			Authenticator authenticator = new Authenticator (clientDriver.communicator);
			authenticator.Authenticated += HandleAuthenticated;
		}

		public event EventHandler Authenticated;

		private void HandleAuthenticated (object sender, EventArgs e)
		{
			worldManager = new WorldManager (clientDriver.communicator);
			worldManager.ReceivedUpdates += new EventHandler<UpdateEventArgs> (HandleReceivedUpdates);
			worldManager.ReceivedNewObject += new EventHandler<NewObjectEventArgs> (HandleReceivedNewObject);
			worldManager.EntityCreated += new EventHandler<EntityCreatedEventArgs> (HandleEntityCreated);
			if (Authenticated != null)
				Authenticated (this, e);
		}

		public event EventHandler<UpdateEventArgs> ReceivedUpdate;

		private void HandleReceivedUpdates (object sender, UpdateEventArgs e)
		{
			if (ReceivedUpdate != null)
				ReceivedUpdate (sender, e);
		}

		public event EventHandler<NewObjectEventArgs> ReceivedNewObject;

		private void HandleReceivedNewObject (object sender, NewObjectEventArgs e)
		{
			if (ReceivedNewObject != null)
				ReceivedNewObject (sender, e);
		}

		public void CreateNewEntity ()
		{
			worldManager.CreateEntity ();           
		}

		public void CreateEntityAt (Vector3 position)
		{
			worldManager.CreateEntityAt (position);
		}

		public void GetDragon ()
		{
			worldManager.GetDragon ();
		}

		public void UpdateBones (EntityInfo e, List<Transform> t)
		{
			worldManager.UpdateBones (e, t);
		}

		public event EventHandler<EntityCreatedEventArgs> EntityCreated;

		private void HandleEntityCreated (object sender, EntityCreatedEventArgs e)
		{
			if (EntityCreated != null)
				EntityCreated (this, e);
		}

		public void MoveEntity (EntityInfo info)
		{
			clientDriver.MoveEntity (info);
		}

		public void RotateEntity (EntityInfo info)
		{
			clientDriver.RotateEntity (info);
		}

		public void Disconnect ()
		{
			if (clientDriver.communicator != null && clientDriver.communicator.IsConnected)
				clientDriver.communicator.CloseConnection ();
		}

		public bool IsConnected {
			get {
				if (clientDriver.communicator != null)
					return clientDriver.communicator.IsConnected;
				else
					return false;
			}
		}

		ClientDriver clientDriver;
		WorldManager worldManager;
	}
}
