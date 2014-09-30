using System;
using System.Collections.Generic;
using WebSocket4Net;

namespace NativeClient
{
    /// <summary>
    /// A call request parameters. Passed to the callbacks registered for a function called by the server.
    /// </summary>
    public class CallRequest
    {
        public CallRequest(string message, List<object> parsedMessage)
        {                        
            Message = message;
            CallID = (int)(long) parsedMessage[1];
            FuncName = (string)parsedMessage[2];
            Callbacks = parsedMessage[3] as List<int>;
            Args = parsedMessage.GetRange(4, parsedMessage.Count - 4);
        }

        public string Message;
        public int CallID;
        public string FuncName;
        public List<int> Callbacks;
        public List<object> Args;
    }

    /// <summary>
    /// A call reply parameters. Passed to the callbacks registered for an expected call reply.
    /// </summary>
    public class CallReply
    {
        public CallReply(string message, List<object> parsedMessage)
        {
            Message = message;
            CallID = (int)(long)parsedMessage[1];
            Success = (bool)parsedMessage[2];
            RetValue = parsedMessage[3];
        }

        public string Message;
        public int CallID;
        public bool Success;
        public object RetValue;
    }

    /// <summary>
    /// Handles the communication with the server.
    /// </summary>
    public class Communicator
    {
        public Communicator() {}        

        /// <summary>
        /// Opens a binary protocol connection.
        /// </summary>
        /// <param name="host">Server host.</param>
        /// <param name="port">Server port.</param>
       /* public void OpenConnection(string host, int port)
        {
            socket = new BPSocketAdapter(host, port);
            Initialize();
        }
        */
        /// <summary>
        /// Opens a WebSocket-JSON protocol connection.
        /// </summary>
        /// <param name="serverURI">Server URI.</param>
        public void OpenConnection(string serverURI)
        {
            socket = new WebSocket(serverURI);
            Initialize();
        }
		public void CloseConnection()
		{
			socket.Close();

		}

        /// <summary>
        /// Occurs when connection is established.
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        /// Occurs when connection is closed.
        /// </summary>
        public event EventHandler Disconnected;

        public bool IsConnected
        {
            get
            {
                return socket.State == WebSocketState.Open;
            }
        }

        /// <summary>
        /// Call the specified funcName with specified args.
        /// </summary>
        /// <param name="funcName">Func name.</param>
        /// <param name="args">Arguments.</param>
        public int Call(string funcName, params object[] args) {
            return Call(funcName, new List<int>(), args);
        }

        /// <summary>
        /// Call the specified funcName with specified callbacks and specified args. The callbacks argument should
        /// contain indicies of arguments that are names of functions registered via RegisterFunc.
        /// </summary>
        /// <param name="funcName">Func name.</param>
        /// <param name="callbacks">Callbacks.</param>
        /// <param name="args">Arguments.</param>
        public int Call(string funcName, List<int> callbacks, params object[] args) {
            int callID = GetNextCallID();
            List<object> message = new List<object>();
            message.Add("call");
            message.Add(callID);
            message.Add(funcName);
            message.Add(callbacks);
            message.AddRange(args);

            var serializedMessage = Json.Serialize(message);
            socket.Send(serializedMessage);

            return callID;
        }

        private int GetNextCallID()
        {
            lock (nextCallIDLock)
                return nextCallID++;
        }

        /// <summary>
        /// Registers the callback for a function. Returns generated function name.
        /// </summary>
        /// <returns>Generated function name.</returns>
        /// <param name="callback">Callback.</param>
        public string RegisterFunc(Action<CallRequest> callback) {
            string name = Guid.NewGuid().ToString();
            RegisterFunc(name, callback);
            return name;
        }

        /// <summary>
        /// Registers the callback for a function with specified name.
        /// </summary>
        /// <param name="funcName">Function name.</param>
        /// <param name="callback">Callback.</param>
        public void RegisterFunc(string funcName, Action<CallRequest> callback) {
            lock (registeredFuncs)
                registeredFuncs.Add(funcName, callback);
        }

        /// <summary>
        /// Adds a callback to be invoked on a call reply with specified ID.
        /// </summary>
        /// <param name="callID">Call reply ID.</param>
        /// <param name="callback">Callback.</param>
        public void AddReplyHandler(int callID, Action<CallReply> callback) {
            lock (expectedReplies)
                expectedReplies.Add(callID, callback);
        }

        void Initialize()
        {
			socket.Opened += (sender, e) => Console.WriteLine("Connected to the server");
			socket.Error += (sender, e) => Console.WriteLine("Connection error", e.Exception);
			socket.Closed += (sender, e) => Console.WriteLine("Connection closed");
            socket.MessageReceived += HandleMessage;
            socket.Opened += HandleOpened;
            socket.Closed += HandleClosed;
            socket.Open();            
        }

        void HandleOpened(object sender, EventArgs e)
        {
            if (Connected != null)
                Connected(this, new EventArgs());
        }

        void HandleClosed(object sender, EventArgs e)
        {
            if (Disconnected != null)
                Disconnected(this, new EventArgs());
        }

        void HandleCallMessage(string message, List<object> parsedMessage)
        {
            string funcName = (string)parsedMessage[2];
            Action<CallRequest> callback;

            lock (registeredFuncs)
            {
                if (!registeredFuncs.ContainsKey(funcName))
                   Console.WriteLine("Unexpected func call: {0}", message);
                callback = registeredFuncs[funcName];
            }

            callback(new CallRequest(message, parsedMessage));
        }

        void HandleCallReplyMessage(string message, List<object> parsedMessage)
        {
            int callID = (int) (long) parsedMessage[1];

            Action<CallReply> callback;
            lock (expectedReplies)
            {
                if (!expectedReplies.ContainsKey(callID))
                    return;
                callback = expectedReplies[callID];
            }

            callback(new CallReply(message, parsedMessage));
        }

        void HandleMessage(object sender, WebSocket4Net.MessageReceivedEventArgs e)
        {
            List<object> parsedMessage = Json.Deserialize(e.Message) as List<object>;
            string messageType = (string)parsedMessage[0];

            try
            {
                if (messageType == "call-error")
					Console.WriteLine("Received error message: {0}", e.Message);
                else if (messageType == "call")
                    HandleCallMessage(e.Message, parsedMessage);
                else if (messageType == "call-reply")
                    HandleCallReplyMessage(e.Message, parsedMessage);
            }
            catch (Exception ex)
            {
				Console.WriteLine("Failed to parse incoming message: " + e.Message, ex);
            }
        }

        /// <summary>
        /// Underlying Web Socket connection.
        /// </summary>
        WebSocket socket;

        /// <summary>
        /// Registered functions to be invoked on call from another side.
        /// </summary>
        Dictionary<string, Action<CallRequest>> registeredFuncs = new Dictionary<string, Action<CallRequest>>();

        /// <summary>
        /// Handlers for expected replies.
        /// </summary>
        Dictionary<int, Action<CallReply>> expectedReplies = new Dictionary<int, Action<CallReply>>();

        /// <summary>
        /// Next call ID. Used to generate unique call IDs.
        /// </summary>
        object nextCallIDLock = new object();
        int nextCallID = 0;        		        
    }
}

