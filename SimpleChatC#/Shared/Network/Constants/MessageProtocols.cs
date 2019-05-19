using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Network.Constants
{
    public enum MessageProtocols
    {
        Setup,
        Register,
        RegisterSuccess,
        LoginSuccess,
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