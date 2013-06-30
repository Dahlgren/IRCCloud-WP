using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRCCLoudLibrary
{
    public class Server
    {
        public int Id { get; set;}
        public String Name { get; set; }
        public String Nick { get; set; }
        public Dictionary<int, Buffer> Buffers { get; private set; }
        public Dictionary<String, Channel> Channels { get; private set; }

        public Server()
        {
            Buffers = new Dictionary<int, Buffer>();
            Channels = new Dictionary<String, Channel>();
        }
    }

    public class Buffer
    {
        public int Id { get; set; }
        public IRCCLoudLibrary.Server Server { get; set; }
        public String Name { get; set; }
        public List<Message> Messages { get; private set; }

        public Buffer()
        {
            Messages = new List<Message>();
        }
    }

    public class Channel
    {
        public IRCCLoudLibrary.Buffer Buffer { get; set; }
        public IRCCLoudLibrary.Server Server { get; set; }
        public String Name { get; set; }
        public String Topic { get; set; }
    }

    public class Message
    {
        public IRCCLoudLibrary.Buffer Buffer { get; set; }
        public IRCCLoudLibrary.Server Server { get; set; }
        public String Msg { get; set; }
        public String User { get; set; }
        public long Timestamp { get; set; }
    }
}
