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
            if (parts[2].StartsWith("#"))
            {
                ircConnection.IrcEvents.OnChannelMessage(parts[2], user, message);
            }
            else
            {
                ircConnection.IrcEvents.OnPrivateMessage(user, message);
            }
        }
    }
}

