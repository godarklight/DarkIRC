using System;

namespace DarkIrc.Messages
{
    public class JoinPart : IMessageHandler
    {
        public void HandleMessage(string rawText, IrcConnection ircConnection)
        {
            string[] parts = rawText.Split(' ');
            string user = parts[0].Substring(1, parts[0].IndexOf("!") - 1);
            if (parts[1] == "JOIN")
            {
                ircConnection.IrcEvents.OnJoin(parts[2], user);
            }
            if (parts[1] == "PART")
            {
                ircConnection.IrcEvents.OnPart(parts[2], user);
            }
        }
    }
}

