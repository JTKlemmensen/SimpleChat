using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Network.Constants
{
    public class MessageProtocols
    {
        public const string Message = "MESSAGE";
        public const string Users = "USERS";
        public const string Connect = "CONNECT";
        public const string Disconnect = "DISCONNECT";
        public const string Pong = "PONG";
        public const string Ping = "PING";
        public const string End = "END";
        public const string SetUsername = "SET_USERNAME";
        public const string UsernameTaken = "USERNAME_TAKEN";
        public const string UsernameChanged = "USERNAME_CHANGED";
        public const string KickUser = "KICK_USER";
    }
}
