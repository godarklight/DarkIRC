using System;

namespace DarkIrc
{
    public class IrcEvents
    {
        public Action<string> Logger;

        public event Action ConnectEvent;
        public event Action DisconnectEvent;
        //Channel, username
        public event Action<string, string> KickEvent;
        //Channel, username
        public event Action<string, string> JoinEvent;
        //Channel, username
        public event Action<string, string> PartEvent;
        //Channel, User, message
        public event Action<string, string, string> ChannelMessageEvent;
        //User, message
        public event Action<string, string> PrivateMessageEvent;
        //Raw Message
        public event Action<string> RawMessageEvent;

        internal void Log(string logText)
        {
            if (Logger != null)
            {
                Logger(logText);
            }
        }

        internal void OnConnect()
        {
            if (ConnectEvent != null)
            {
                ConnectEvent();
            }
        }

        internal void OnDisconnect()
        {
            if (DisconnectEvent != null)
            {
                DisconnectEvent();
            }
        }

        internal void OnJoin(string channel, string user)
        {
            if (JoinEvent != null)
            {
                JoinEvent(channel, user);
            }
        }

        internal void OnKick(string channel, string user)
        {
            if (KickEvent != null)
            {
                KickEvent(channel, user);
            }
        }

        internal void OnPart(string channel, string user)
        {
            if (PartEvent != null)
            {
                PartEvent(channel, user);
            }
        }

        internal void OnRawMessage(string message)
        {
            if (RawMessageEvent != null)
            {
                RawMessageEvent(message);
            }
        }

        internal void OnChannelMessage(string channel, string user, string message)
        {
            if (ChannelMessageEvent != null)
            {
                ChannelMessageEvent(channel, user, message);
            }
        }

        internal void OnPrivateMessage(string user, string message)
        {
            if (PrivateMessageEvent != null)
            {
                PrivateMessageEvent(user, message);
            }
        }
    }
}

