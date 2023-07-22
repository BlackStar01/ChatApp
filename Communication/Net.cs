using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Communication
{
    public class Net
    {
        [Serializable]
        public class PrivateMessage : Message
        {
            public string Sender { get; set; }
            public string Receiver { get; set; }
            public string Content { get; set; }

            public PrivateMessage(string sender, string receiver, string content)
            {
                Sender = sender;
                Receiver = receiver;
                Content = content;
            }

            public override string ToString()
            {
                return $"{Sender} to {Receiver}: {Content}";
            }
        }
        public static void sendMsg(Stream s, Message msg)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(s, msg);
        }

        public static Message rcvMsg(Stream s)
        {
            BinaryFormatter bf = new BinaryFormatter();
            return (Message)bf.Deserialize(s);
        }
    }
}
