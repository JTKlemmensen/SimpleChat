using Shared;
using SharedCode;
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
        private IdleChecker idle;
        private AsymmetricCipher asymCipher;
        private int v;

        public SimpleClient(string ip, int port, string username = "")
        {
            try
            {
                Socket connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                connection.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                this.Start(connection);
                SetupConnection(username);

                idle = new IdleChecker(this);
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
                case MessageProtocols.Message:
                    if (message.Arguments.Count() == 2)
                        NewMessage?.Invoke(message.Arguments[0],message.Arguments[1]);
                    break;

                case MessageProtocols.Users:
                    TotalUsers?.Invoke(message.Arguments);
                    ConnectionConnect?.Invoke();
                    break;

                case MessageProtocols.Connect:
                    if (message.Arguments.Count() == 1)
                        UserConnected?.Invoke(message.Arguments[0]);
                    break;

                case MessageProtocols.Disconnect:
                    if (message.Arguments.Count() == 1)
                        UserDisconnected?.Invoke(message.Arguments[0]);
                    break;

                case MessageProtocols.Pong:
                    if (idle != null)
                        idle.Pong();
                    break;

                case MessageProtocols.Ping:
                    Send(MessageProtocols.Pong, true);
                    break;

                case MessageProtocols.End:
                    Terminate();
                    break;

                case MessageProtocols.SetUsername:
                    if (message.Arguments.Count == 1)
                    {
                        SetUsername?.Invoke(message.Arguments[0]);
                    }
                    break;

                case MessageProtocols.UsernameTaken:
                    UsernameTaken?.Invoke();
                    break;

                case MessageProtocols.UsernameChanged:
                    if (message.Arguments.Count == 2)
                    {
                        UsernameChanged?.Invoke(message.Arguments[0], message.Arguments[1]);
                    }
                    break;

            }
        }

        public void SendMessage(string message)
        {
            Send(MessageProtocols.Message, true, message);
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

        public delegate void OnSetUsername(string username);
        public event OnSetUsername SetUsername;

        public delegate void OnUsernameTaken();
        public event OnUsernameTaken UsernameTaken;

        public delegate void OnUsernameChanged(string oldUsername, string changedUsername);
        public event OnUsernameChanged UsernameChanged;

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

        private void SetupConnection(string username = "")
        {
            asymCipher = new AsymmetricCipher();
            Send(asymCipher.PublicKey(), false);
        }

        public override void Terminate()
        {
            Send(MessageProtocols.End, true);
            base.Terminate();
        }
    }
}