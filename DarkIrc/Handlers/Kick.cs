using System;

namespace DarkIrc.Messages
{
    public class Kick : IMessageHandler
    {
        public void HandleMessage(string rawText, IrcConnection ircConnection)
        {
            string actualMessage = rawText;
            if (rawText.StartsWith(":"))
            {
                actualMessage = rawText.Substring(rawText.IndexOf(" ") + 1);
            }
            string[] parts = actualMessage.Split(' ');
            ircConnection.IrcEvents.OnKick(parts[1], parts[2]);
        }
    }
}

