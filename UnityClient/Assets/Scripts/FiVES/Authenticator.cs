using System;
using System.Collections.Generic;

namespace NativeClient
{
    /// <summary>
    /// Handles client authentication.
    /// </summary>
    public class Authenticator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NativeClient.Authenticator"/> class. The Communication must
        /// be connected.
        /// </summary>
        /// <param name="Communicator">The Communicator instance to be used to communicate with the server.</param>
        public Authenticator(Communicator communicator)
        {
            if (!communicator.IsConnected)
                throw new ArgumentException("Communicator must be connected when passed to Authenticator constructor.");

            Communicator = communicator;
            QueryAuthInterface();
        }

        /// <summary>
        /// Occurs when the client is authenticated.
        /// </summary>
        public event EventHandler Authenticated;

        void QueryAuthInterface()
        {
            int callID = Communicator.Call("kiara.implements", new List<string> { "auth" });
            Communicator.AddReplyHandler(callID, HandleQueryAuthInterfaceReply);
        }

        void HandleQueryAuthInterfaceReply(CallReply reply)
        {
            if (!reply.Success)
				Console.WriteLine("Failed to request auth service: {0}", reply.Message);

            List<object> retValue = (List<object>)reply.RetValue;
            if (!(bool) retValue[0])
				Console.WriteLine("No auth service.");

            int callID = Communicator.Call("auth.login", GenerateRandomLogin(), "");
            Communicator.AddReplyHandler(callID, HandleAuthorizationReply);
        }

        void HandleAuthorizationReply(CallReply reply)
        {
            if (!reply.Success)
				Console.WriteLine("Failed on authentication: {0}", reply.Message);

            bool success = (bool)reply.RetValue;
            if (!success)
                Console.WriteLine("Incorrect login/password", reply.Message);

            if (Authenticated != null)
                Authenticated(this, new EventArgs());
        }

        string GenerateRandomLogin()
        {
            var randomizer = new Random();
            return "user" + randomizer.Next();
        }

        Communicator Communicator;
	        
    }
}

