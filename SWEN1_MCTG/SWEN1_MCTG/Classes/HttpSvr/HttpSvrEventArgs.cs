using System;
using System.Net.Sockets;
using System.Text;



namespace SWEN1_MCTG.Classes.HttpSvr
{
    /// <summary>This class defines event arguments for the <see cref="HttpSvrEventHandler"/> event handler.</summary>
    public class HttpSvrEventArgs: EventArgs
    {
        // Members   
        /// <summary> TCP client. </summary>
        protected TcpClient _Client;

        // Constructor
        /// <summary> Creates a new instance of this class. </summary>
        /// <param name="client"> TCP client. </param>
        /// <param name="plainMessage"> Plain HTTP message. </param>
        public HttpSvrEventArgs(TcpClient client, string plainMessage) 
        {
            _Client = client;

            PlainMessage = plainMessage;
            Payload = string.Empty;

            string[] lines = plainMessage.Replace("\r\n", "\n").Split('\n');
            bool inheaders = true;
            List<HttpHeader> headers = new();

            for(int i = 0; i < lines.Length; i++) 
            {
                if(i == 0)
                {
                    string[] inc = lines[0].Split(' ');
                    Method = inc[0];
                    Path = inc[1];
                    continue;
                }

                if(inheaders)
                {
                    if(string.IsNullOrWhiteSpace(lines[i])) 
                    {
                        inheaders = false;
                    }
                    else { headers.Add(new(lines[i])); }
                }
                else
                {
                    if(!string.IsNullOrWhiteSpace(Payload)) { Payload += "\r\n"; }
                    Payload += lines[i];
                }
            }

            Headers = headers.ToArray();
        }

        // Properties
        /// <summary> Gets the plain message. </summary>
        public string PlainMessage
        {
            get; protected set;
        } = string.Empty;


        /// <summary> Gets the HTTP method. </summary>
        public virtual string Method
        {
            get; protected set;
        } = string.Empty;


        /// <summary> Gets the HTTP path. </summary>
        public virtual string Path
        {
            get; protected set;
        } = string.Empty;


        /// <summary> Gets the HTTP headers. </summary>
        public virtual HttpHeader[] Headers
        {
            get; protected set;
        } = Array.Empty<HttpHeader>();


        /// <summary> Gets the payload. </summary>
        public virtual string Payload
        {
            get; protected set;
        } = string.Empty;

        // Methods
        /// <summary>
        /// Sends an HTTP response to the client with the specified status code and optional body content.
        /// </summary>
        /// <param name="status">The HTTP status code to send in the response.</param>
        /// <param name="body">The optional body content to include in the response. If null or empty, the response will have a Content-Length of 0.</param>
        /// <remarks>
        /// This method constructs an HTTP response message based on the provided status code and body content.
        /// It writes the response to the client's network stream and then closes and disposes of the client connection.
        /// </remarks>
        public void Reply(int status, string? body = null)
        {
            string data;

            switch(status)
            {
                case 200:
                    data = "HTTP/1.1 200 OK\n"; break;
                case 400:
                    data = "HTTP/1.1 400 Bad Request\n"; break;
                case 401:
                    data = "HTTP/1.1 401 Unauthorized\n"; break;
                case 404:
                    data = "HTTP/1.1 404 Not found\n"; break;
                default:
                    data = $"HTTP/1.1 {status} Status unknown\n"; break;
            }

            if(string.IsNullOrEmpty(body)) 
            {
                data += "Content-Length: 0\n";
            }
            data += "Content-Type: text/plain\n\n";
            if(!string.IsNullOrEmpty(body)) { data += body; }

            byte[] buf = Encoding.ASCII.GetBytes(data);
            _Client.GetStream().Write(buf, 0, buf.Length);
            _Client.Close();
            _Client.Dispose();
        }
    }
}
