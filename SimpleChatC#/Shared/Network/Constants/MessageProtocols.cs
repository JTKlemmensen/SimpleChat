using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Network.Constants
{
    public enum MessageProtocols
    {
        Setup,
        Login,
        Fail,
        Message,
        Users,
        Connect,
        Disconnect,
        Pong,
        Ping,
        End,
        SetUsername,
        UsernameTaken,
        UsernameChanged,
        KickUser
    }
}