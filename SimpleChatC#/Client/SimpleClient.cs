using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class SimpleClient : Connection
    {
        private AsymmetricCipher asymCipher;
        private int v;

        public SimpleClient(string ip, int port)
        {
            Socket connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
                connection.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                this.Start(connection);

                SetupConnection();   
        }

        public SimpleClient(int v)
        {
            this.v = v;
        }

        protected override void Command(NetworkMessage message)
        {
            if (!HasEstablishedConnection())
            {
                EstablishConnection(message);
                return;
            }

            switch(message.Protocol)
            {
                case "MESSAGE":
                    if (message.Arguments.Count() == 2)
                        NewMessage?.Invoke(message.Arguments[0],message.Arguments[1]);
                    break;
            }
        }

        public void SendMessage(string message)
        {
            Send("MESSAGE", true, message);
        }

        public delegate void OnNewMessage(string message, string sender);
        public event OnNewMessage NewMessage;

        public delegate void OnUserConnected(string user);
        public event OnUserConnected UserConnected;

        public delegate void OnUserDisconnected(string user);
        public event OnUserDisconnected UserDisconnected;

        private bool HasEstablishedConnection()
        {
            return cipher != null;
        }

        private void EstablishConnection(NetworkMessage message)
        {
            try
            {
                Console.WriteLine("Establish Connection");
                cipher = new SymmetricCipher();
                Console.WriteLine("Establish Connection1");

                string Key = asymCipher.Decrypt(message.Protocol);
                Console.WriteLine("Establish Connection2");
                Console.WriteLine(message.Arguments[0]);

                string IV = asymCipher.Decrypt(message.Arguments[0]);

                Console.WriteLine("KEY: " + Key);
                Console.WriteLine("IV;; " + IV);

                cipher.Key = Key;
                cipher.IV = IV;

                Console.WriteLine("Has established connection!");
            }
            catch (Exception)
            {
                Terminate();
            }
        }

        private void SetupConnection()
        {
            asymCipher = new AsymmetricCipher();

            Console.WriteLine("SETUP CONNECTION");
            Send(asymCipher.PublicKey(), false);
            Console.WriteLine("SETUP CONNECTION2");
        }
    }
}