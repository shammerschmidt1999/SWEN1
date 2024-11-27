using System;



namespace SWEN1_MCTG.Classes.HttpSvr
{
    // Represents an HTTP header
    public class HttpHeader
    {
        // Constructor
        public HttpHeader(string header)
        {
            Name = Value = string.Empty;

            try
            {
                int n = header.IndexOf(':');
                Name = header.Substring(0, n).Trim();
                Value = header.Substring(n + 1).Trim();
            }
            catch(Exception) {}
        }

        // Properties
        public string Name
        {
            get; protected set;
        }

        public string Value
        {
            get; protected set;
        }
    }
}
