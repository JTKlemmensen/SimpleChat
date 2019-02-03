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
            try
            {
                Socket connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                connection.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                this.Start(connection);
                SetupConnection();
            }
            catch (Exception){}
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

                case "USERS":
                    TotalUsers?.Invoke(message.Arguments);
                    ConnectionConnect?.Invoke();
                    break;

                case "CONNECT":
                    if (message.Arguments.Count() == 1)
                        UserConnected?.Invoke(message.Arguments[0]);
                    break;

                case "DISCONNECT":
                    if (message.Arguments.Count() == 1)
                        UserDisconnected?.Invoke(message.Arguments[0]);
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

        public delegate void OnTotalUsers(IEnumerable<string> users);
        public event OnTotalUsers TotalUsers;

        public delegate void OnUserDisconnected(string user);
        public event OnUserDisconnected UserDisconnected;
        
        public delegate void OnConnectionConnect();
        public event OnConnectionConnect ConnectionConnect;

        private bool HasEstablishedConnection()
        {
            return cipher != null;
        }

        private void EstablishConnection(NetworkMessage message)
        {
            try
            {
                cipher = new SymmetricCipher();
                string Key = asymCipher.Decrypt(message.Protocol);
                string IV = asymCipher.Decrypt(message.Arguments[0]);

                cipher.Key = Key;
                cipher.IV = IV;
            }
            catch (Exception)
            {
                Terminate();
            }
        }

        private void SetupConnection()
        {
            asymCipher = new AsymmetricCipher();
            Send(asymCipher.PublicKey(), false);
        }

        public override void Terminate()
        {
            Send("END",true);
            base.Terminate();
        }
    }
}