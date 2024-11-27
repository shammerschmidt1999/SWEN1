using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;


namespace SWEN1_MCTG.Classes.HttpSvr
{
    /// <summary>This class defines event arguments for the <see cref="HttpSvrEventHandler"/> event handler.</summary>
    public class HttpSvrEventArgs: EventArgs
    {
        // TCP client
        protected TcpClient _Client;

        // Constructor
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
        public string PlainMessage
        {
            get; protected set;
        } = string.Empty;

        public virtual string Method
        {
            get; protected set;
        } = string.Empty;

        public virtual string Path
        {
            get; protected set;
        } = string.Empty;

        public virtual HttpHeader[] Headers
        {
            get; protected set;
        } = Array.Empty<HttpHeader>();

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

            switch (status)
            {
                case HttpStatusCode.OK:
                    data = $"HTTP/1.1 {HttpStatusCode.OK} OK\n";
                    break;
                case HttpStatusCode.BAD_REQUEST:
                    data = $"HTTP/1.1 {HttpStatusCode.BAD_REQUEST} Bad Request\n";
                    break;
                case HttpStatusCode.UNAUTHORIZED:
                    data = $"HTTP/1.1 {HttpStatusCode.UNAUTHORIZED} Unauthorized\n";
                    break;
                case HttpStatusCode.NOT_FOUND:
                    data = $"HTTP/1.1 {HttpStatusCode.NOT_FOUND} Not Found\n";
                    break;
                default:
                    data = $"HTTP/1.1 {status} Status Unknown\n";
                    break;
            }

            string message = string.Empty;
            if (!string.IsNullOrEmpty(body))
            {
                try
                {
                    JsonNode? json = JsonNode.Parse(body);
                    if (json != null)
                    {
                        if (json["message"] != null)
                        {
                            message = json["message"]!.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing body: {ex.Message}");
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                data += $"Message: {message}\n";
            }

            if (string.IsNullOrEmpty(body))
            {
                data += "Content-Length: 0\n";
            }
            data += "Content-Type: text/plain\n\n";
            if (!string.IsNullOrEmpty(body))
            {
                data += body;
            }

            data += "\n";

            byte[] buf = Encoding.ASCII.GetBytes(data);
            _Client.GetStream().Write(buf, 0, buf.Length);
            _Client.Close();
            _Client.Dispose();
        }


    }
}
