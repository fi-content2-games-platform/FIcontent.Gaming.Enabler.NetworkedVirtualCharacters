using System;
using System.Collections.Generic;

namespace NativeClient
{
	public class ClientDriver
	{

		public ClientDriver ()
		{
			communicator = new Communicator ();
		}

		/// <summary>
		/// Starts the simulation of the client.
		/// </summary>
		public void Connect ()
		{
			if (ServerURI != null)
				communicator.OpenConnection (ServerURI);
			/*
            else
                communicator.OpenConnection(ServerHost, ServerPort);*/
		}

		void HandleDisconnected (object sender, EventArgs e)
		{
			Environment.Exit (-1);
		}

		public void MoveEntity (EntityInfo info)
		{
			communicator.Call ("location.updatePosition", info.Guid, info.Position, Timestamps.UnixTimestamp);
		}

		public void RotateEntity (EntityInfo info)
		{
			communicator.Call ("location.updateOrientation", info.Guid, info.Orientation, Timestamps.UnixTimestamp);
		}

#pragma warning disable 414
		string ServerURI = null;
		string ServerHost = "127.0.0.1";
		int ServerPort = 34837;
		bool EnableMovement = true;
		bool EnableRotation = true;
		int NumEntitiesToGenerate = 1;
		int ActionDelayMs = 250;
#pragma warning restore 414

		public Communicator communicator;		        
	}
}
