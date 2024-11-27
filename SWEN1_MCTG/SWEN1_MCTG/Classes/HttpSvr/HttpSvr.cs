using System;
using System.Net;
using System.Net.Sockets;
using System.Text;



namespace SWEN1_MCTG.Classes.HttpSvr
{
    // Implements an HTTP server
    public sealed class HttpSvr
    {
        
        // Properties
        private TcpListener? _Listener;

        // Gets if the server is available
        public bool Active
        {
            get; private set;
        } = false;

        // Events
        // Is raised when incoming data is available
        public event HttpSvrEventHandler? Incoming;

        // Methods
        /// <summary>
        /// Starts the HTTP server and listens for incoming TCP connections.
        /// </summary>
        /// <remarks>
        /// This method initializes the TCP listener and starts it on the local IP address (127.0.0.1) and port 12000.
        /// It continuously accepts incoming TCP connections and reads data from them.
        /// When data is available, it raises the Incoming event with the received data.
        /// The server remains active until the Stop method is called.
        /// </remarks>
        public void Run()
        {
            if(Active) return;

            Active = true;
            _Listener = new(IPAddress.Parse("127.0.0.1"), 10001);
            _Listener.Start();

            byte[] buf = new byte[256];

            while(Active)
            {
                TcpClient client = _Listener.AcceptTcpClient();
                string data = string.Empty;
                
                while(client.GetStream().DataAvailable || string.IsNullOrWhiteSpace(data))
                {
                    int n = client.GetStream().Read(buf, 0, buf.Length);
                    data += Encoding.ASCII.GetString(buf, 0, n);
                }

                Incoming?.Invoke(this, new(client, data));
            }
        }


        /// <summary>
        /// Stops the server
        /// </summary>
        public void Stop()
        {
            Active = false;
        }
    }
}
