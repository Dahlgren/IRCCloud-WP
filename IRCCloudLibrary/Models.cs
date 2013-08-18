using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace IRCCloudLibrary
{
    public class Server
    {
        public int Id { get; set;}
        public String Name { get; set; }
        public String Nick { get; set; }
        public Dictionary<int, Buffer> Buffers { get; private set; }
        public Dictionary<String, Channel> Channels { get; private set; }
        public SortedObservableCollection<Buffer> SortedBuffers { get; private set; }

        public Server()
        {
            Buffers = new Dictionary<int, Buffer>();
            Channels = new Dictionary<String, Channel>();
            SortedBuffers = new SortedObservableCollection<Buffer>();
        }

        internal void AddBuffer(Buffer buffer)
        {
            if (!Buffers.ContainsKey(buffer.Id) || Buffers[buffer.Id] == null)
            {
                Buffers[buffer.Id] = buffer;
                SortedBuffers.Add(buffer);
            }
            else
            {
                Buffer existingBuffer = Buffers[buffer.Id];

                existingBuffer.Name = buffer.Name;
                existingBuffer.Archived = buffer.Archived;
            }
        }
    }

    public class Buffer : IComparable
    {
        public int Id { get; set; }
        public Server Server { get; set; }
        public String Name { get; set; }
        public String Type { get; set; }
        public SortedObservableCollection<Message> Messages { get; private set; }
        public Boolean Archived { get; set; }

        private Dictionary<long, Message> SeenMessages;

        public Buffer()
        {
            Messages = new SortedObservableCollection<Message>();
            SeenMessages = new Dictionary<long, Message>();
        }

        public int CompareTo(object obj)
        {
            if (!(obj is Buffer)) {
                throw new NotImplementedException();
            }

            Buffer otherBuffer = obj as Buffer;

            if (Archived != otherBuffer.Archived)
            {
                if (Archived)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }

            return Name.CompareTo(otherBuffer.Name);
        }

        internal void AddMessage(Message message)
        {
            if (!SeenMessages.ContainsKey(message.Timestamp))
            {
                Messages.Add(message);
                SeenMessages.Add(message.Timestamp, message);
            }
        }
    }

    public class Channel
    {
        public Buffer Buffer { get; set; }
        public Server Server { get; set; }
        public String Name { get; set; }
        public String Topic { get; set; }
    }

    public class Message : IComparable
    {
        public Buffer Buffer { get; set; }
        public Server Server { get; set; }
        public String Msg { get; set; }
        public String User { get; set; }
        public long Timestamp { get; set; }

        public int CompareTo(object obj)
        {
            if (!(obj is Message))
            {
                throw new NotImplementedException();
            }

            Message otherMsg = obj as Message;

            return Timestamp.CompareTo(otherMsg.Timestamp);
        }
    }
}
