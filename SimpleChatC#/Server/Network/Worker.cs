using Server;
using Server.Entities;
using Server.Repositories;
using Shared.Ciphers;
using Shared.Network;
using Shared.Network.Constants;
using Shared.Network.Entities;
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
        public User User { get; private set; }
        public string Username { get; private set; }
        private SimpleServer server;
        private IdleChecker idle;

        public Worker(SimpleServer server, Socket connection, string username) : base()
        {
            
            this.AddCommand(MessageProtocols.Register, RegisterCommand);
            this.AddCommand(MessageProtocols.Login, LoginCommand);
            this.AddCommand(MessageProtocols.Message, MessageCommand);
            this.AddCommand(MessageProtocols.End, EndCommand);
            this.AddCommand(MessageProtocols.Ping, PingCommand);
            this.AddCommand(MessageProtocols.Pong, PongCommand);
            
            //this.AddCommand(MessageProtocols.SetUsername, SetUsernameCommand);

            this.server = server;
            this.Username = username;
            this.Start(connection);
            idle = new IdleChecker(this);
        }
        #region incoming commands from client
        private void RegisterCommand(NetworkMessage message)
        {
            if (message.TryGetObject<RegisterRequest>(out RegisterRequest rl))
            {
                try
                {
                    User user = RepositoryFactory.UserRepository.Register(new User { Username = rl.Username, Password = rl.Password });

                    OnUserLoggedIn(user);
                }
                catch (UnsecurePasswordException)
                {
                    this.Send(MessageProtocols.Fail, ResponseCodes.Unsecure_Password);
                }
                catch (UsernameIsTakenException)
                {
                    this.Send(MessageProtocols.Fail, ResponseCodes.Username_Taken);
                }
            }
        }

        private void LoginCommand(NetworkMessage message)
        {
            if (message.TryGetObject<LoginRequest>(out LoginRequest lr))
            {
                User user = RepositoryFactory.UserRepository.Login(lr.Username,lr.Password);

                if(user == null)
                    this.Send(MessageProtocols.Fail, ResponseCodes.Bad_Login);
                else
                    OnUserLoggedIn(user);
            }
        }

        private void MessageCommand(NetworkMessage message)
        {
            if (message.TryGetObject<string>(out string m))
                server.SendMessage(m, Username);
        }

        private void EndCommand(NetworkMessage message)
        {
            Terminate();
        }

        private void PingCommand(NetworkMessage message)
        {
            Send(MessageProtocols.Pong, true);
        }

        private void PongCommand(NetworkMessage message)
        {
            if (idle != null)
                idle.Pong();
        }
        #endregion

        private void OnUserLoggedIn(User user)
        {
            User = user;
            LoginSuccess success = new LoginSuccess();
            success.Users = server.ConnectedUsers();

            Send(MessageProtocols.LoginSuccess, success);
            //TODO Tell other clients another user has logged in
        }


        protected override void EstablishConnection(NetworkMessage message)
        {
            //Generate symmetric key + IV, save it and encrypt it with the given public assymetric key and send it to the client.   
            try
            {
                if (message.TryGetObject<string>(out string publicKey))
                {
                    AsymmetricCipher asymmetric = new AsymmetricCipher();
                    asymmetric.LoadPublicKey(publicKey);
                    cipher = new SymmetricCipher();

                    SymmetricKey key = new SymmetricKey { Key = asymmetric.Encrypt(cipher.Key), IV = asymmetric.Encrypt(cipher.IV) };

                    Send(MessageProtocols.Setup, key, false);
                    Send(MessageProtocols.SetUsername, Username);
                    server.Broadcast(MessageProtocols.Connect, this, Username); // Tell users that it connected
                    Send(MessageProtocols.Users, server.ConnectedUsers().ToArray());
                } else
                    throw new Exception("Failed to load the public key");
            }
            catch (Exception)
            {
                Terminate();
            }
        }

        public override void Terminate()
        {
            Send(MessageProtocols.End);
            server.Broadcast(MessageProtocols.Disconnect, this, Username);
            base.Terminate();
            server.Remove(this);
        }
    }
}