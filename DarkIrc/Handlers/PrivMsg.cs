using System;

namespace DarkIrc.Messages
{
    public class PrivMsg : IMessageHandler
    {
        public void HandleMessage(string rawText, IrcConnection ircConnection)
        {
            string[] parts = rawText.Split(' ');
            string user = parts[0].Substring(1, parts[0].IndexOf("!") - 1);
            string message = rawText.Substring(rawText.IndexOf("PRIVMSG"));
            message = message.Substring(message.IndexOf(":") + 1);
            bool isCtcp = false;
            bool isAction = false;
            if (message.StartsWith((char)1 + "ACTION "))
            {
                isAction = true;
                message = message.Substring(message.IndexOf(" ") + 1);
            }
            else
            {
                if (message[0] == (char)1)
                {
                    message = message.Substring(1);
                    isCtcp = true;
                }
            }
            if (parts[2].StartsWith("#"))
            {
                if (isAction)
                {
                    ircConnection.IrcEvents.OnChannelActionMessage(parts[2], user, message);
                }
                if (isCtcp)
                {
                    ircConnection.IrcEvents.OnChannelCtcpMessage(parts[2], user, message);
                }
                if (!isAction && !isCtcp)
                {
                    ircConnection.IrcEvents.OnChannelMessage(parts[2], user, message);
                }
            }
            else
            {
                if (isAction)
                {
                    ircConnection.IrcEvents.OnPrivateActionMessage(user, message);
                }
                if (isCtcp)
                {
                    ircConnection.IrcEvents.OnPrivateCtcpMessage(user, message);
                }
                if (!isCtcp && !isAction)
                {
                    ircConnection.IrcEvents.OnPrivateMessage(user, message);
                }
            }
        }
    }
}

