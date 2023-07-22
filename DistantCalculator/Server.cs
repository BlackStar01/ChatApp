using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Communication;

namespace DistantCalculator
{
    public class Server
    {
        private int port;

        public Server(int port)
        {
            this.port = port; 
        }
        private Dictionary<string, TcpClient> connectedClients = new Dictionary<string, TcpClient>();
        public void start()
        {
            TcpListener l = new TcpListener(new IPAddress(new byte[] { 127, 0, 0, 1 }), port);
            l.Start();

            while (true)
            {
                TcpClient client = l.AcceptTcpClient();
                Console.WriteLine("Connection established @" + client);
                new Thread(() => HandleClientConnection(client)).Start();

            }
             
        }

        private void HandleClientConnection(TcpClient client)
        {
            string clientName = null;

            try
            {
                NetworkStream stream = client.GetStream();
                while (true)
                {
                    Message msg = (Message)Net.RcvMsg(stream);

                    if (msg is PrivateMessage privateMsg)
                    {
                        if (connectedClients.ContainsKey(privateMsg.Receiver))
                        {
                            TcpClient receiverClient = connectedClients[privateMsg.Receiver];
                            Net.SendMsg(receiverClient.GetStream(), privateMsg);
                        }
                        else
                        {
                            // Handle case where receiver is not connected
                            Console.WriteLine($"User '{privateMsg.Receiver}' is not connected.");
                        }
                    }
                    else if (msg is UserProfile userProfile)
                    {
                        if (UserProfileManager.CheckCredentials(userProfile.Username, userProfile.Password))
                        {
                            clientName = userProfile.Username;
                            connectedClients.Add(clientName, client);
                            Console.WriteLine($"User '{clientName}' connected.");
                        }
                        else
                        {
                            // Handle invalid credentials
                            Console.WriteLine("Invalid login attempt.");
                            Net.SendMsg(stream, new Result(0, true));
                        }
                    }
                    else if (msg is Expr expr)
                    {
                        // Handle mathematical expression
                        // ... (code existant)
                    }
                }
            }
            catch (IOException)
            {
                // Handle disconnection
                Console.WriteLine($"User '{clientName}' disconnected.");
                connectedClients.Remove(clientName);
            }
        }

        class Receiver
        {
            private TcpClient comm;

            public Receiver(TcpClient s)
            {
                comm = s;
            }

            public void doOperation()
            {

                /*
                BinaryWriter outs = new BinaryWriter(comm.GetStream());
                BinaryReader ins = new BinaryReader(comm.GetStream());

                
                // read operation
                double op1 = ins.ReadDouble();
                double op2 = ins.ReadDouble();
                char op = ins.ReadChar();
                */

                Console.WriteLine("Computing operation");
                while (true)
                {
                    // read expression
                    Expr msg = (Expr)Net.rcvMsg(comm.GetStream());

                    Console.WriteLine("expression received");
                    // send result
                    switch (msg.Op)
                    {
                        case '+':
                            Net.sendMsg(comm.GetStream(), new Result(msg.Op1 + msg.Op2, false));
                            break;

                        case '-':
                            Net.sendMsg(comm.GetStream(), new Result(msg.Op1 - msg.Op2, false));
                            break;

                        case '*':
                            Net.sendMsg(comm.GetStream(), new Result(msg.Op1 * msg.Op2, false));
                            break;

                        case '/':
                            if (msg.Op2 != 0.0)
                                Net.sendMsg(comm.GetStream(), new Result(msg.Op1 / msg.Op2, false));
                            else
                                Net.sendMsg(comm.GetStream(), new Result(0, true));

                            break;

                    }

                }

            }
        }
        
        
    }
}
