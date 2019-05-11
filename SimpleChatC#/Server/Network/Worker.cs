using Server;
using Shared.Ciphers;
using Shared.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Worker : Connection
    {
        public string Username { get; private set; }
        private SimpleServer server;
        private IdleChecker idle;

        public Worker(SimpleServer server, Socket connection, string username)
        {
            this.server = server;
            this.Username = username;
            this.Start(connection);
            idle = new IdleChecker(this);
        }

        protected override void OnCommand(NetworkMessage message)
        {
            if (!HasEstablishedConnection())
                EstablishConnection(message);

            switch(message.Protocol)
            {
                case MessageProtocols.Message:
                    if (message.Arguments.Count == 1)
                        server.SendMessage(message.Arguments[0], Username);
                    break;

                case MessageProtocols.End:
                    Terminate();
                    break;

                case MessageProtocols.Ping:
                    Send(MessageProtocols.Pong, true);
                    break;

                case MessageProtocols.Pong:
                    if (idle != null)
                        idle.Pong();
                    break;

                case MessageProtocols.SetUsername:
                    if (message.Arguments.Count == 1)
                    {
                        
                        var usernameToUse = message.Arguments[0];
                        if (server.IsUsernameTaken(usernameToUse))
                        {
                            Send(MessageProtocols.UsernameTaken);
                        }
                        else
                        {
                            var oldUsername = Username;
                            Username = usernameToUse;
                            server.UsernameWasChanged(oldUsername, Username);
                            server.Broadcast(MessageProtocols.UsernameChanged, oldUsername, Username);
                        }
                    }
                    break;

                default:
                    Console.WriteLine(message.Protocol);
                    break;
            }
        }

        private bool HasEstablishedConnection()
        {
            return cipher != null;
        }

        private void EstablishConnection(NetworkMessage message)
        {
            //Generate symmetric key + IV, save it and encrypt it with the given public assymetric key and send it to the client.   
            try
            {
                AsymmetricCipher asymmetric = new AsymmetricCipher();
                asymmetric.LoadPublicKey(message.Protocol);   
                cipher = new SymmetricCipher();

                Send(asymmetric.Encrypt(cipher.Key), false, asymmetric.Encrypt(cipher.IV));
                Send(MessageProtocols.SetUsername, true, Username);
                server.Broadcast(MessageProtocols.Connect, this, Username); // Tell users that it connected
                Send(MessageProtocols.Users, true, server.ConnectedUsers().ToArray());
            }
            catch (Exception)
            {
                //Terminate();
            }
        }

        public override void Terminate()
        {
            Send(MessageProtocols.End, true);
            server.Broadcast(MessageProtocols.Disconnect, this, Username);
            base.Terminate();
            server.Remove(this);
        }
    }
}